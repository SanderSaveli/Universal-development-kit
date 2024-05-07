using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SceneLoad
{
    [System.Serializable]
    public struct Transition
    {
        public float duration;
        public Material material;
    }

    public class TransitionAnimator : MonoBehaviour, ITransitionAnimator
    {
        [SerializeField] private GameObject transitionCanvasPrefab;
        public List<Transition> transitions = new List<Transition>();

        private Material maskMaterial;
        private float maskAmount = 0f;
        private float targetValue = 1f;
        private Image transitionImage;
        private Coroutine coroutine;
        private string canvasName;

        private void OnEnable()
        {
            canvasName = transitionCanvasPrefab.name;
            SceneManager.sceneLoaded += CreateCanvasInNewScene;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= CreateCanvasInNewScene;
        }

        private void Start()
        {
            if (transitions.Count == 0)
            {
                Debug.LogWarning("There is no one transition!");
            }
        }

        private IEnumerator PlayAnimation(float duration)
        {
            transitionImage.enabled = true;
            while (Mathf.Abs(targetValue - maskAmount) > 0.05f)
            {
                float maskAmountChange = targetValue > maskAmount ? 1f : -1f;
                maskAmount += maskAmountChange * Time.deltaTime * (1 / duration);
                maskAmount = Mathf.Clamp(maskAmount, -0.1f, 1);

                maskMaterial.SetFloat("_MaskAmount", maskAmount);
                yield return null;
            }
            transitionImage.enabled = false;
        }

        public void PlayTransistAnimation(int transitionIndex, out float animationDuration)
        {
            targetValue = 1f;
            maskAmount = -.1f;
            Transition transition = GetTransition(transitionIndex);
            animationDuration = transition.duration;
            if(transition.material != null)
            {
                maskMaterial = transition.material;
                transitionImage.material = maskMaterial;
                StartAnimationCorutine(transition);
            }
        }

        public void PlayTransistAnimationReverse(int transitionIndex, out float animationDuration)
        {
            targetValue = -.1f;
            maskAmount = 1;
            Transition transition = GetTransition(transitionIndex);
            animationDuration = transition.duration;
            if (transition.material != null)
            {
                maskMaterial = transition.material;
                transitionImage.material = maskMaterial;
                StartAnimationCorutine(transition);
            }
        }

        private void StartAnimationCorutine(Transition transition)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(PlayAnimation(transition.duration));
        }

        private Transition GetTransition(int transitionIndex)
        {
            if (transitions.Count == 0)
            {
                Debug.LogWarning($"There is no one transition, played default transition!");
                return default(Transition);
            }
            if(transitions.Count <= transitionIndex)
            {
                Debug.LogWarning($"There is no transition with index {transitionIndex}, played {transitions.Count - 1} transition!");

            }
            int index = Mathf.Clamp(transitionIndex, 0, transitions.Count - 1);
            return transitions[index];
        }

        private void CreateCanvasInNewScene(Scene arg0, LoadSceneMode arg1)
        {
            GameObject canvasObj = GameObject.Find(canvasName);
            if (canvasObj == null)
            {
                canvasObj = Instantiate(transitionCanvasPrefab);
            }
            transitionImage = canvasObj.GetComponentInChildren<Image>();
            transitionImage.enabled = false;
        }
    }
}

