using System.Collections.Generic;
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
    public static CameraFollow CameraFollow
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

    private static Camera _camera;
    public static Camera Camera
    {
        get
        {
            if (_camera == null)
            {
                _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            }
            return _camera;
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

    private static LeaderboardManager _leaderboardManager;
    public static LeaderboardManager LeaderboardManager
    {
        get
        {
            if (_leaderboardManager == null)
            {
                _leaderboardManager = GameObject.Find("LeaderboardManager").GetComponent<LeaderboardManager>();
            }

            return _leaderboardManager;
        }
    }

    private static Transform _collectionTargetPos;
    public static Transform CollectionTargetPos
    {
        get
        {
            if (_collectionTargetPos == null)
            {
                _collectionTargetPos = GameObject.Find("CollectionTargetPos").transform;
            }

            return _collectionTargetPos;
        }
    }

    private static Transform _canvas;
    public static Transform Canvas
    {
        get
        {
            if (_canvas == null)
            {
                _canvas = GameObject.Find("Canvas").transform;
            }

            return _canvas;
        }
    }

    private static Dictionary<SkinType, GameObject> _helicoptorBodies = new Dictionary<SkinType, GameObject>();
    public static GameObject GetHelicoptorBody(SkinType skinType)
    {
        if (!_helicoptorBodies.ContainsKey(skinType))
        {
            _helicoptorBodies[skinType] = Resources.Load<GameObject>($"Helicoptors/{skinType}");
        }

        return _helicoptorBodies[skinType];
    }
}
