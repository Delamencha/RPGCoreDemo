using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        public IEnumerator LoadLastScene(string saveFile)
        {
            // get state
            // load last scnene
            // restore state
            Dictionary<string, object> state = LoadFile(saveFile);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                buildIndex = (int)state["lastSceneBuildIndex"];

            }
            yield return SceneManager.LoadSceneAsync(buildIndex);

            //潜在发生data hazard可能，当loadSceneAsync不执行时，RestoreState在Awake中执行，而在Awake中可能未配置Mover.Restore()所需的NavMeshAgent，从而error
            //故修改为无论if值，都执行LoadSceneAsync
            RestoreState(state);

        }

        public void Save(string saveFile)
        {
            Dictionary<string, object>  state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }



        public void Load(string saveFile)
        {

            RestoreState(LoadFile(saveFile));

        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);

            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                
                BinaryFormatter fofmatter = new BinaryFormatter();
                fofmatter.Serialize(stream, state);
            }
        } 

        public void DeleteSaveFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);

            if(File.Exists(path)) File.Delete(path);

            print("delete savefile done");

        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path)) return new Dictionary<string, object>();

            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }


        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {               
                state[saveable.GetUniqueIdentifier()] = saveable.CaptrueState();  
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state)
        {
       
            foreach(SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveable.Restore(state[id]);
                }
                    
            }

        } 

        private string GetPathFromSaveFile(string saveFile)
        {


            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }

    }
}

