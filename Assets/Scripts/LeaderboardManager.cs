using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    void Awake()
    {
        PlayGamesPlatform.Activate();
        AutoLogin();
    }

    private void AutoLogin()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("login success");
            }
        });
    }

    public void PostScore(int distance)
    {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ReportScore(
                distance,
                GPGSIds.leaderboard_furthest,
                (success) =>
                {
                    if (success) Debug.Log("Score posted to leaderboard " + GameState.Player.HighestDistanceUnlocked);
                    else Debug.Log("Score leaderboard post failed " + GameState.Player.HighestDistanceUnlocked);
                });
        }
    }

    public void OpenLeaderboard()
    {
        if (!Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ManuallyAuthenticate((SignInStatus status) =>
            {
                if (status == SignInStatus.Success)
                {
                    PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_furthest);
                }
                else
                {
                    Debug.LogError("Google Failed to Authorize your login");
                }
            });
        }
        else
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_furthest);
        }
    }
}