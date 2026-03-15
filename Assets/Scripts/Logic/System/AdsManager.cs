using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener
{
    public static AdsManager Instance { get; private set; }

#if UNITY_IOS
    const string _gameId = "6065783";
    const string _bannerAdUnitId = "v6idyclwhampyrr8";
#else
    const string _gameId = "6065782";
    const string _bannerAdUnitId = "v6idyclwhampyrr8";
#endif

    [SerializeField] bool _testMode = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

    // ── IUnityAdsInitializationListener ──

    public void OnInitializationComplete()
    {
        Debug.Log("[AdsManager] Unity Ads 초기화 완료");
        LoadBanner();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"[AdsManager] Unity Ads 초기화 실패: {error} - {message}");
    }

    // ── 배너 로드 / 표시 ──

    void LoadBanner()
    {
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Load(_bannerAdUnitId,
            new BannerLoadOptions
            {
                loadCallback = OnBannerLoaded,
                errorCallback = OnBannerLoadFailed
            });
    }

    void OnBannerLoaded()
    {
        Debug.Log("[AdsManager] 배너 광고 로드 완료");
        ShowBanner();
    }

    void OnBannerLoadFailed(string message)
    {
        Debug.LogError($"[AdsManager] 배너 광고 로드 실패: {message}");
    }

    /// <summary>하단 배너 광고를 표시합니다.</summary>
    public void ShowBanner()
    {
        Advertisement.Banner.Show(_bannerAdUnitId,
            new BannerOptions
            {
                showCallback = OnBannerShown,
                hideCallback = OnBannerHidden,
                clickCallback = OnBannerClicked
            });
    }

    /// <summary>배너 광고를 숨깁니다.</summary>
    public void HideBanner()
    {
        Advertisement.Banner.Hide();
        Debug.Log("[AdsManager] 배너 광고 숨김");
    }

    void OnBannerShown() => Debug.Log("[AdsManager] 배너 광고 표시됨");
    void OnBannerHidden() => Debug.Log("[AdsManager] 배너 광고 숨겨짐");
    void OnBannerClicked() => Debug.Log("[AdsManager] 배너 광고 클릭됨");

    // ── IUnityAdsLoadListener (배너 외 광고 유닛 확장 시 사용) ──

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"[AdsManager] 광고 로드 완료: {placementId}");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"[AdsManager] 광고 로드 실패: {placementId} - {error} - {message}");
    }
}
