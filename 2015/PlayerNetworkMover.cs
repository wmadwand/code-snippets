using UnityEngine;
using CompleteProject;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;

//namespace UnityStandardAssets.Characters.FirstPerson
//{


public class PlayerNetworkMover : Photon.MonoBehaviour
{
    public delegate void Respawn(float time);
    public event Respawn RespawnMe;

    public delegate void SendMessagePlayer(string message);
    public event SendMessagePlayer SendNetworkMessage;

    Vector3 position;
    Quaternion rotation;
    float smoothing = 10f;
    int health = 100;
    bool aim = false;
    bool sprint = false;
    Animator anim;
    string textMeshText;

    PlayerHealth_ok playerHealth;

    bool initialLoad = true;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        textMeshText = GetComponentInChildren<TextMesh>().text;
        playerHealth = GetComponent<PlayerHealth_ok>();
        //plNameLableQuat = transform.Find("PlayerNameLable").gameObject.transform.rotation;

        if (photonView.isMine)
        {
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<CharacterController>().enabled = true;
            GetComponent<FirstPersonController_ok>().enabled = true;
            GetComponent<PlayerHealth_ok>().enabled = true;
            GetComponent<PlayerHealth_ok>().healthSlider.value = health;

            GetComponent<AudioSource>().enabled = true;
            //AudioSource.PlayClipAtPoint(spawnSound, spawnPoints[spawnPointIndex].position);

            GetComponentInChildren<PlayerShooting_ok>().enabled = true;
            GetComponentInChildren<ShootingForCamera>().enabled = true;

            foreach (Camera cam in GetComponentsInChildren<Camera>())
            {
                cam.enabled = true;
            }

            foreach (CapsuleCollider capCol in GetComponentsInChildren<CapsuleCollider>())
            {
                capCol.enabled = true;
            }

            foreach (AudioSource audio in GetComponentsInChildren<AudioSource>())
            {
                audio.enabled = true;
            }

            foreach (AudioListener audioListener in GetComponentsInChildren<AudioListener>())
            {
                audioListener.enabled = true;
            }

            transform.Find("FirstPersonCharacter/GunCamera/Gun").gameObject.layer = 9;
            transform.Find("FirstPersonCharacter/GunCamera/Gun/GunBarrelEnd").gameObject.layer = 9;
            transform.Find("FirstPersonCharacter/GunCamera/Gun/Sights").gameObject.layer = 9;
        }

        else
        {
            StartCoroutine("UpdateData");
        }

    }

    IEnumerator UpdateData()
    {
        if (initialLoad)
        {
            initialLoad = false;
            transform.position = position;
            transform.rotation = rotation;
        }

        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
            anim.SetBool("Aim", aim);
            anim.SetBool("Sprint", sprint);
            GetComponentInChildren<TextMesh>().text = textMeshText;

            yield return null;
        }

    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(health);
            stream.SendNext(anim.GetBool("Aim"));
            stream.SendNext(anim.GetBool("Sprint"));
            stream.SendNext(textMeshText);
        }

        else
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
            //playerHealth.currentHealth = (int)stream.ReceiveNext();
            health = (int)stream.ReceiveNext();
            aim = (bool)stream.ReceiveNext();
            sprint = (bool)stream.ReceiveNext();
            textMeshText = (string)stream.ReceiveNext();

        }
    }

    [PunRPC]
    public void GetShot(int takeDamage, string enemyName)
    {
        health -= takeDamage;

        if (photonView.isMine)
        {
            //playerHealth.healthSlider.value = health;
            playerHealth.TakeDamage(takeDamage);
        }

        if (health <= 0 && photonView.isMine)
        {
            if (SendNetworkMessage != null)
            {
                SendNetworkMessage(PhotonNetwork.player.name + " was killed by " + enemyName);
            }

            if (RespawnMe != null)
            {
                RespawnMe(3f);
            }

            PhotonNetwork.Destroy(gameObject);
            //Debug.Log("Died");
        }
    }


}
//}
