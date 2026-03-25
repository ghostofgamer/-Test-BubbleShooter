using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Screens
{
    public class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen Instance { get; private set; }

        [Header("UI Elements")] [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField] private Slider progressBar;

        [Header("Settings")] [SerializeField] private float fadeSpeed = 3f;
        [SerializeField] private float progressSpeed = 1.5f;
        [SerializeField] private float minLoadTime = 0.5f;

        private Coroutine _loadingCoroutine;
        private bool _isLoading = false;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void OnDisable()
        {
            _cts?.Cancel();
        }

        public void LoadScene(string sceneName)
        {
            progressBar.value = 0;

            if (_isLoading) return;

            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            Load(sceneName, _cts.Token).Forget();
        }

        private async UniTask Load(string sceneName, CancellationToken ct)
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                await FadeIn(ct);
                AsyncOperationHandle<SceneInstance> handle =
                    Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single, activateOnLoad: false);

                float fakeProgress = 0f;
                float elapsed = 0f;

                while (!handle.IsDone || elapsed < minLoadTime)
                {
                    ct.ThrowIfCancellationRequested();
                    elapsed += Time.deltaTime;
                    float targetProgress = Mathf.Clamp01(handle.PercentComplete);
                    fakeProgress = Mathf.MoveTowards(fakeProgress, targetProgress, progressSpeed * Time.deltaTime);

                    if (progressBar != null)
                        progressBar.value = fakeProgress;

                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                await handle.Task;
                handle.Result.ActivateAsync();

                if (progressBar != null)
                    progressBar.value = 1f;

                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: ct);

                await FadeOut(ct);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Загрузка сцены отменена");
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка: " + ex.Message + "\n" + ex.StackTrace);
                Debug.Log("Произошла ошибка: " + ex.Message);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async UniTask FadeIn(CancellationToken ct)
        {
            gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = true;

            while (canvasGroup.alpha < 1f)
            {
                ct.ThrowIfCancellationRequested();

                canvasGroup.alpha += Time.deltaTime * fadeSpeed;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            canvasGroup.alpha = 1f;
        }

        private async UniTask FadeOut(CancellationToken ct)
        {
            while (canvasGroup.alpha > 0f)
            {
                ct.ThrowIfCancellationRequested();

                canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }
    }
}