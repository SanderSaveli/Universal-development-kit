using UnityEngine;

namespace SceneLoad
{
    public class SceneLoaderTest : MonoBehaviour
    {
        SceneLoader loader;

        private void Start()
        {
            loader = SceneLoader.instance;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                loader.LoadScene("SampleScene", 0, 1);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                loader.LoadScene("test", 0, 1);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                loader.LoadSceneWithLoadScreen("test", "testLoadAnimation", 0, 1, 2, 3);
            }
        }
    }

}

