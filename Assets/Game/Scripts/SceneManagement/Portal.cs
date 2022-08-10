using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Saving;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        { 
            A, B, C, D, E
        }


        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeTime = 2f;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                StartCoroutine(Transition());
                
            }

        }

        private IEnumerator Transition()
        {
            if(sceneToLoad < 0)
            {
                Debug.LogError("scene to load not set");
                yield break;
            }
           DontDestroyOnLoad(gameObject);

            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            
            Fader fader = FindObjectOfType<Fader>();

            //Remove control
            PlayerController control = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            control.enabled = false;

            yield return fader.FadeOut(fadeTime);

            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            //remove new control
            PlayerController newControl = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newControl.enabled = false;

            wrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            wrapper.Save();

            yield return new WaitForSeconds(fadeTime);
            fader.FadeIn(fadeTime);

            //restore control
            newControl.enabled = true;

            Destroy(gameObject);
            
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;
                return portal;
            }
            return null;
        }
    }
}

