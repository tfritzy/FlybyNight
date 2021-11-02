using UnityEngine;

public static class Managers
{
    private static Helicopter helicopter;
    public static Helicopter Helicopter
    {
        get
        {
            if (helicopter == null)
            {
                helicopter = GameObject.Find("Helicopter").GetComponent<Helicopter>();
            }
            return helicopter;
        }
    }

    private static UIManager uIManager;
    public static UIManager UIManager
    {
        get
        {
            if (uIManager == null)
            {
                uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
            }
            return uIManager;
        }
    }

    private static CameraFollow camera;
    public static CameraFollow Camera
    {
        get
        {
            if (camera == null)
            {
                camera = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
            }
            return camera;
        }
    }

    private static FireworkShooter fireworkShooter;
    public static FireworkShooter FireworkShooter
    {
        get
        {
            if (fireworkShooter == null)
            {
                fireworkShooter = GameObject.Find("FireworkShooter").GetComponent<FireworkShooter>();
            }

            return fireworkShooter;
        }
    }

    private static GridManager gridManager;
    public static GridManager GridManager
    {
        get
        {
            if (gridManager == null)
            {
                gridManager = GameObject.Find("Grid").GetComponent<GridManager>();
            }

            return gridManager;
        }
    }

    private static FadeToBlack fadeToBlackScreen;
    public static FadeToBlack FadeToBlackScreen
    {
        get
        {
            if (fadeToBlackScreen == null)
            {
                fadeToBlackScreen = GameObject.Find("FadeToBlack").GetComponent<FadeToBlack>();
            }

            return fadeToBlackScreen;
        }
    }

    private static Backdrop backdrop;
    public static Backdrop Backdrop
    {
        get
        {
            if (backdrop == null)
            {
                backdrop = GameObject.Find("Backdrop").GetComponent<Backdrop>();
            }

            return backdrop;
        }
    }

    // private static GPGManager gpgManager;
    // public static GPGManager GPGManager
    // {
    //     get
    //     {
    //         if (gpgManager == null)
    //         {
    //             gpgManager = GameObject.Find("LeaderboardManager").GetComponent<GPGManager>();
    //         }

    //         return gpgManager;
    //     }
    // }
}