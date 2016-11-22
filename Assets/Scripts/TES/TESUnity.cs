using System.IO;
using TESUnity.UI;
using UnityEngine;

namespace TESUnity
{
    public class TESUnity : MonoBehaviour
    {
        public static TESUnity instance;

        public enum MWMaterialType
        {
            Default, Standard, BumpedDiffuse, Unlit
        }

        #region Inspector-set Members

#if UNITY_EDITOR
        [Header("Editor Only")]
        [SerializeField]
        private bool _bypassINIConfig = false;
#endif

        [Header("Global")]
        public string dataPath;
        public bool useKinematicRigidbodies = true;
        public bool playMusic = true;

        [Header("Rendering")]
        public MWMaterialType materialType = MWMaterialType.BumpedDiffuse;
        public RenderingPath renderPath = RenderingPath.Forward;

        [Header("Lighting")]
        public float ambientIntensity = 1.5f;
        public bool renderSunShadows = false;
        public bool renderLightShadows = false;
        public bool renderExteriorCellLights = false;
        public bool animateLights = false;

        [Header("Effects")]
        public bool antiAliasing = false;
        public bool ambientOcclusion = false;
        public bool bloom = false;
        public bool waterBackSideTransparent = false;

        [Header("VR")]
        public bool followHeadDirection = false;
        public bool directModePreview = true;

        [Header("UI")]
        public UIManager UIManager;
        public Sprite UIBackgroundImg;
        public Sprite UICheckmarkImg;
        public Sprite UIDropdownArrowImg;
        public Sprite UIInputFieldBackgroundImg;
        public Sprite UIKnobImg;
        public Sprite UIMaskImg;
        public Sprite UISpriteImg;

        [Header("Prefabs")]
        public GameObject playerPrefab;
        public GameObject waterPrefab;

        [Header("Debug")]
        public bool creaturesEnabled = false;
        public bool npcsEnabled = false;

        #endregion

        private MorrowindDataReader MWDataReader;
        private MorrowindEngine MWEngine;
        private MusicPlayer musicPlayer;

        public MorrowindEngine Engine
        {
            get { return MWEngine; }
        }

        public TextureManager TextureManager
        {
            get { return MWEngine.textureManager; }
        }

        private void Awake()
        {
            instance = this;

            var path = dataPath;
#if UNITY_EDITOR
            if (!_bypassINIConfig)
                path = GameSettings.CheckSettings(this);
#else
            var path = GameSettings.CheckSettings(this);
#endif

            if (!GameSettings.IsValidPath(path))
            {
                GameSettings.SetDataPath(string.Empty);
                UnityEngine.SceneManagement.SceneManager.LoadScene("AskPathScene");
            }
            else
                dataPath = path;
        }

        private void Start()
        {
            MWDataReader = new MorrowindDataReader(dataPath);
            MWEngine = new MorrowindEngine(MWDataReader, UIManager);

            if (playMusic)
            {
                // Start the music.
                musicPlayer = new MusicPlayer();

                foreach (var songFilePath in Directory.GetFiles(dataPath + "/Music/Explore"))
                    if (!songFilePath.Contains("Morrowind Title"))
                        musicPlayer.AddSong(songFilePath);

                musicPlayer.Play();
            }

            // Spawn the player.
            //MWEngine.SpawnPlayerInside(playerPrefab, new Vector2i(4537908, 1061158912), new Vector3(0.8f, -0.45f, -1.4f));

            MWEngine.SpawnPlayerOutside(playerPrefab, new Vector2i(-2, -9), new Vector3(-137.94f, 2.30f, -1037.6f));
        }

        private void OnDestroy()
        {
            if (MWDataReader != null)
            {
                MWDataReader.Close();
                MWDataReader = null;
            }
        }

        private void Update()
        {
            MWEngine.Update();

            if (playMusic)
                musicPlayer.Update();

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (MWEngine.currentCell == null || !MWEngine.currentCell.isInterior)
                    Debug.Log(MWEngine.GetExteriorCellIndices(Camera.main.transform.position));
                else
                    Debug.Log(MWEngine.currentCell.NAME.value);
            }
#endif
        }

#if UNITY_EDITOR
        private void TestAllCells(string resultsFilePath)
        {
            using (StreamWriter writer = new StreamWriter(resultsFilePath))
            {
                foreach (var record in MWDataReader.MorrowindESMFile.GetRecordsOfType<ESM.CELLRecord>())
                {
                    var CELL = (ESM.CELLRecord)record;

                    try
                    {
                        var cellInfo = MWEngine.InstantiateCell(CELL);
                        MWEngine.temporalLoadBalancer.WaitForTask(cellInfo.creationCoroutine);
                        DestroyImmediate(cellInfo.gameObject);

                        writer.Write("Pass: ");
                    }
                    catch
                    {
                        writer.Write("Fail: ");
                    }

                    if (!CELL.isInterior)
                    {
                        writer.WriteLine(CELL.gridCoords.ToString());
                    }
                    else
                    {
                        writer.WriteLine(CELL.NAME.value);
                    }
                }
            }
        }
#endif
    }
}