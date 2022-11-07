using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Effects
{
    public class LaserBeam2 : MonoBehaviour
    {

        private LineRenderer lr;
        // Use this for initialization
        void Start()
        {
            lr = GetComponent<LineRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            lr.SetPosition(0, transform.position);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit))
            {
                if (hit.collider)
                {
                    lr.SetPosition(1, hit.point);
                }
            }
            else lr.SetPosition(1, transform.forward * 5000);
        }
    }
}