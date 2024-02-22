using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public interface INavMeshAgentTransform
{
    bool Valid { get; }
    bool CanMove { get; }
	bool FreezeRotation { get; }
	Vector2 Position { get; set; }
	Vector2 LookAtPosition { get; set; }
	bool ShouldReset { get; }
}

public class NavAgent
{
    float MoveDistance;
    IList<Vector2> ActivePath;
    int CurrentMoveIndex = -1;
    INavMeshAgentTransform Transform;
    public bool HasPath { get { return ActivePath != null; } }
    System.Action OnStopMove;

    public int GetPathIndex() { return CurrentMoveIndex; }
    public float GetMoveDistance() { return MoveDistance; }
    public void ResetMoveDistance() { MoveDistance = 0; }
    public Vector3? GetLocalDestination(int offset = 0)
    {
        var index = CurrentMoveIndex + offset;
        if (ActivePath == null || index < 0 || index >= ActivePath.Count)
        {
            return null;
        }
        return ActivePath[index];
    }

    public NavAgent(INavMeshAgentTransform transform)
    {
        Transform = transform;
    }

    public bool SetPath(IList<Vector2> newPath)
    {
        if (ActivePath != null) { return false; }
        ActivePath = newPath;
        CurrentMoveIndex = 0;
        return true;
    }


    public void StopMove(System.Action onStopMove = null)
    {
        if (ActivePath != null)
        {
            ActivePath = null;
            OnStopMove = onStopMove;
        }
        else
        {
            onStopMove?.Invoke();
        }

    }

    public IEnumerator MoveByPath(IList<Vector2> newPath, float distance)
    {
        OnStopMove = null;
        if (newPath.Count == 0)
        {
            yield break;
        }
        ActivePath = newPath;
        CurrentMoveIndex = 0;
        var origin = Transform.Position;

        while (ActivePath != null)
        {
            yield return null;
            if (!Transform.Valid)
            {
                yield break;
            }
            if (Transform.ShouldReset)
            {
                Transform.Position = origin;
                break;
            }
            UpdatePosition(distance);
        }
        OnStopMove?.Invoke();
    }


    public void UpdatePosition(float distance)
    {
        if (ActivePath == null) { return; }
        if (CurrentMoveIndex >= ActivePath.Count || CurrentMoveIndex < 0)
        {
            ActivePath = null;
            CurrentMoveIndex = -1;
            return;
        }

        if (!Transform.CanMove) { return; }

        var targetPoint = ActivePath[CurrentMoveIndex];
        Transform.LookAtPosition = targetPoint;
        var a = targetPoint - Transform.Position;
        var magnitude = a.magnitude;
        if (magnitude <= distance)
        {
            MoveDistance += magnitude;
            distance -= magnitude;
            Transform.Position = targetPoint;
            CurrentMoveIndex++;
            if (CurrentMoveIndex >= ActivePath.Count)
            {
                ActivePath = null;
                CurrentMoveIndex = -1;
            }
            else
            {
                UpdatePosition(distance);
            }
        }
        else
        {
            MoveDistance += distance;
            Transform.Position += a / magnitude * distance;
        }
    }
}