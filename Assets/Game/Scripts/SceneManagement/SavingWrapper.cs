using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float fadeInTime = 1f;
        const string defaultSaveFile = "save";

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
            
        }

        private IEnumerator LoadLastScene()
        {

            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);

            Fader fader = FindObjectOfType<Fader>();

            fader.FadeOutImmediate();

            yield return fader.FadeIn(fadeInTime);

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                //不用loadLastScene：在play中不会发生，当前所在scene必然有一个保存；loadLastScene只在editor进入play时发生
                Load();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                DeleteSaving();
            }
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void DeleteSaving()
        {
            GetComponent<SavingSystem>().DeleteSaveFile(defaultSaveFile);
        }

    }
}


