using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField]
        Text connectionText;
        [SerializeField]
        Transform[] spawnPoints;
        [SerializeField]
        Camera sceneCamera;

        [SerializeField]
        GameObject serverWindow;
        [SerializeField]
        GameObject textTitle;
        [SerializeField]
        GameObject textScore;
        [SerializeField]
        InputField userName;
        [SerializeField]
        InputField roomName;
        [SerializeField]
        InputField roomList;
        [SerializeField]
        InputField messageWindow;

        [SerializeField]
        GameObject gameController;

        [SerializeField]
        AudioClip spawnSound;

        GameObject player;
        Queue<string> messages;
        const int messageCount = 6;
        PhotonView photonView;

        void Start()
        {
            photonView = GetComponent<PhotonView>();
            messages = new Queue<string>(messageCount);

            PhotonNetwork.logLevel = PhotonLogLevel.Full;
            PhotonNetwork.ConnectUsingSettings("1.0");
            StartCoroutine("UpdateConnectionString");
        }

        IEnumerator UpdateConnectionString()
        {
            while (true)
            {
                connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();
                yield return null;
            }
        }

        void OnJoinedLobby()
        {
            serverWindow.SetActive(true);
        }

        void OnReceivedRoomListUpdate()
        {
            roomList.text = "";
            RoomInfo[] rooms = PhotonNetwork.GetRoomList();
            foreach (RoomInfo room in rooms)
            {
                roomList.text += room.name + "\n";
            }
        }

        public void JoinRoom()
        {
            PhotonNetwork.player.name = userName.text;
            RoomOptions myRoom = new RoomOptions() { isVisible = true, maxPlayers = 10 };
            PhotonNetwork.JoinOrCreateRoom(roomName.text, myRoom, TypedLobby.Default);
        }

        void OnJoinedRoom()
        {
            serverWindow.SetActive(false);
            textTitle.SetActive(false);
            StopCoroutine("UpdateConnectionString");
            connectionText.text = "";
            gameController.GetComponent<GameController>().DisableCursor();
            gameController.GetComponent<GameController>().ActivateUI();
            gameController.GetComponent<AudioSource>().Play();
            textScore.SetActive(false);
            StartRespawnProcess(0f);
        }

        void StartRespawnProcess(float respawnTime)
        {
            sceneCamera.enabled = true;
            StartCoroutine("SpawnPlayer", respawnTime);
        }

        void ShowSpawnEffect(int spawnPointIndex, float ySpawn)
        {
            PhotonNetwork.Instantiate("LightningSpark", new Vector3(spawnPoints[spawnPointIndex].position.x, ySpawn, spawnPoints[spawnPointIndex].position.z), spawnPoints[spawnPointIndex].rotation, 0);
            AudioSource.PlayClipAtPoint(spawnSound, spawnPoints[spawnPointIndex].position);
        }

        IEnumerator SpawnPlayer(float respawnTime)
        {
            yield return new WaitForSeconds(respawnTime);
            int index = Random.Range(0, spawnPoints.Length);

            ShowSpawnEffect(index, 2f);
            yield return new WaitForSeconds(0.5f);

            player = PhotonNetwork.Instantiate("FPSPLayer", spawnPoints[index].position, spawnPoints[index].rotation, 0);
            player.GetComponentInChildren<TextMesh>().text = PhotonNetwork.playerName;
            player.GetComponent<PlayerNetworkMover>().RespawnMe += StartRespawnProcess;
            player.GetComponent<PlayerNetworkMover>().SendNetworkMessage += AddMessage;

            sceneCamera.enabled = false;

            AddMessage("Spawned player: " + PhotonNetwork.player.name);

            yield return null;
        }

        void AddMessage(string message)
        {
            photonView.RPC("AddMessage_RPC", PhotonTargets.All, message);
        }

        [PunRPC]
        void AddMessage_RPC(string message)
        {
            messages.Enqueue(message);
            if (messages.Count > messageCount)
            {
                messages.Dequeue();
            }

            messageWindow.gameObject.SetActive(true);
            messageWindow.text = "";
            foreach (string m in messages)
            {
                messageWindow.text += m + "\n";
            }
        }
    }
}