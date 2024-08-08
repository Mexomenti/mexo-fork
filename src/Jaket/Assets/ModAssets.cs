namespace Jaket.Assets;

using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

using Jaket.Content;
using Jaket.Net;
using Jaket.Net.Types;
using Jaket.UI.Dialogs;
using Train;

/// <summary> Class that works with the assets bundle of the mod. </summary>
public class ModAssets
{
    /// <summary> Bundle containing assets for player doll. </summary>
    public static AssetBundle Bundle;

    /// <summary> Player doll and its preview prefabs. </summary>
    public static GameObject Doll, Preview;

    /// <summary> Jaket plushies. </summary>
    public static GameObject V2, V3;

    /// <summary> Player doll icon. </summary>
    public static Sprite Icon;

    /// <summary> Mixer processing Sam's voice. Used to change volume. </summary>
    public static AudioMixer Mixer;

    /// <summary> Font used by the mod. Differs from the original in support of Cyrillic alphabet. </summary>
    public static Font Font;
    public static TMP_FontAsset FontTMP;

    /// <summary> Shader used by the game for materials. </summary>
    public static Shader Shader;

    /// <summary> Wing textures used to differentiate teams. </summary>
    public static Texture[] WingTextures;

    /// <summary> Hand textures used by local player. </summary>
    public static Texture[] HandTextures;
    // <summary> Body textures used by local player. </summary>
    public static Texture[] BodyTextures;

    /// <summary> Coin texture used by team coins. </summary>
    public static Texture CoinTexture;

    /// <summary> Icons for the emoji selection wheel. </summary>
    public static Sprite[] EmojiIcons, EmojiGlows;

    /// <summary> Loads assets bundle and other necessary stuff. </summary>
    public static void Load()
    {
        Bundle = LoadBundle();

        // cache the shader and the wing textures for future use
        Shader = AssetHelper.LoadPrefab("cb3828ada2cbefe479fed3b51739edf6").GetComponent<global::V2>().smr.material.shader;
        WingTextures = new Texture[Tools.EnumMax<Team>() + 1];
        HandTextures = new Texture[6];
        BodyTextures = new Texture[3];

        // loading wing textures from the bundle
        for (int i = 0; i < WingTextures.Length; i++)
        {
            var index = i; // C# sucks
            LoadAsync<Texture>("V3-wings-" + ((Team)i).ToString(), tex => WingTextures[index] = tex);
        }

        LoadAsync<Texture>("v3-body", tex => BodyTextures[0] = tex);
        LoadAsync<Texture>("v3-body-blue", tex => BodyTextures[1] = tex);
        LoadAsync<Texture>("v3-body-red", tex => BodyTextures[2] = tex);

        LoadAsync<Texture>("V3-hand", tex => HandTextures[1] = tex);
        LoadAsync<Texture>("V3-blast", tex => HandTextures[3] = tex);
        LoadAsync<Texture>("V3-hand-red", tex => HandTextures[4] = tex);
        LoadAsync<Texture>("V3-blast-blue", tex => HandTextures[5] = tex);
        HandTextures[0] = FistControl.Instance.blueArm.ToAsset().GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture;
        HandTextures[2] = FistControl.Instance.redArm.ToAsset().GetComponentInChildren<SkinnedMeshRenderer>().material.mainTexture;

        LoadAsync<Texture>("coin", tex => CoinTexture = tex);

        // load icons for emoji wheel
        EmojiIcons = new Sprite[12];
        EmojiGlows = new Sprite[12];

        for (int i = 0; i < 12; i++)
        {
            var index = i;
            LoadAsync<Sprite>("V3-emoji-" + i, tex => EmojiIcons[index] = tex);
            LoadAsync<Sprite>("V3-emoji-" + i + "-glow", tex => EmojiGlows[index] = tex);
        }

        // create prefabs of the player doll and its preview
        LoadAsync<GameObject>("Player Doll.prefab", prefab =>
        {
            Object.DontDestroyOnLoad(prefab);
            FixMaterials(prefab);

            Doll = prefab;
        });

        LoadAsync<GameObject>("Player Doll Preview.prefab", prefab =>
        {
            Object.DontDestroyOnLoad(prefab);
            FixMaterials(prefab);

            Preview = prefab;
        });

        // I guess async will improve performance a little bit
        LoadAsync<Sprite>("V3-icon", sprite => Icon = sprite);
        LoadAsync<AudioMixer>("sam-audio", mix =>
        {
            Mixer = mix;
            Events.Post(() =>
            {
                Networking.LocalPlayer.Voice.outputAudioMixerGroup = Mixer.FindMatchingGroups("Master")[0];
            });
        });

        // but the font must be loaded immediately, because it is needed to build the interface
        Font = Bundle.LoadAsset<Font>("font.ttf");
        FontTMP = TMP_FontAsset.CreateFontAsset(Font);

        LoadAsync<Texture>("V2-plushie", t =>
        {
            int i = EntityType.V2 - EntityType.ItemOffset;
            Object.DontDestroyOnLoad(V2 = Items.Prefabs[i] = Object.Instantiate(Items.Prefabs[i]));

            V2.name = "DevPlushie (V2)";
            V2.GetComponentInChildren<Renderer>().material.mainTexture = t;
            V2.GetComponent<Rigidbody>().isKinematic = true;
        });

        LoadAsync<Texture>("V3-plushie", t =>
        {
            int i = EntityType.V3 - EntityType.ItemOffset;
            Object.DontDestroyOnLoad(V3 = Items.Prefabs[i] = Object.Instantiate(Items.Prefabs[i]));

            V3.name = "DevPlushie (V3)";
            V3.GetComponentInChildren<Renderer>().material.mainTexture = t;
            V3.GetComponent<Rigidbody>().isKinematic = true;
        });
    }

    /// <summary> Finds and loads an assets bundle. </summary>
    public static AssetBundle LoadBundle()
    {
        string assembly = Plugin.Instance.Location;
        string directory = Path.GetDirectoryName(assembly);
        string bundle = Path.Combine(directory, "jaket-assets.bundle");

        return AssetBundle.LoadFromFile(bundle);
    }

    /// <summary> Finds and asynchronously loads an asset. </summary>
    public static void LoadAsync<T>(string name, UnityAction<T> cons) where T : Object
    {
        var task = Bundle.LoadAssetAsync<T>(name);
        task.completed += _ => cons(task.asset as T);
    }

    /// <summary> Changes the colors of materials and their shaders to match the style of the game.. </summary>
    public static void FixMaterials(GameObject obj)
    {
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>(true))
        {
            // component responsible for drawing the trace
            if (renderer is TrailRenderer) continue;

            // body, rocket & hook materials
            foreach (var mat in renderer.materials)
            {
                mat.color = Color.white;
                mat.shader = Shader;
            }
        }
    }

    /// <summary> Tags after loading from a bundle changes due to a mismatch in the tags list, this method returns everything to its place. </summary>
    public static string MapTag(string tag) => tag switch
    {
        "RoomManager" => "Body",
        "Body" => "Limb",
        "Forward" => "Head",
        _ => tag
    };

    /// <summary> Creates a new player doll from the prefab. </summary>
    public static RemotePlayer CreateDoll()
    {
        // create a doll from the prefab obtained from the bundle
        var obj = Entities.Mark(Doll);

        // add components
        var enemyId = obj.AddComponent<EnemyIdentifier>();
        var machine = obj.AddComponent<Machine>();

        enemyId.enemyClass = EnemyClass.Machine;
        enemyId.enemyType = EnemyType.V2;
        enemyId.dontCountAsKills = true;
        enemyId.weaknesses = new string[0];
        enemyId.burners = new();
        enemyId.activateOnDeath = new GameObject[0];
        machine.destroyOnDeath = new GameObject[0];
        machine.hurtSounds = new AudioClip[0];

        // add enemy identifier to all doll parts so that bullets can hit it
        foreach (var rigidbody in obj.transform.GetChild(0).GetComponentsInChildren<Rigidbody>())
        {
            rigidbody.gameObject.AddComponent<EnemyIdentifierIdentifier>();
            rigidbody.tag = MapTag(rigidbody.gameObject.tag);
        }

        // add a script to further control the doll
        return obj.AddComponent<RemotePlayer>();
    }

    /// <summary> Returns the hand texture currently in use. Depends on whether the player is in the lobby or not. </summary>
    /// <summary> handType == 0, feedbacker </summary>
    /// <summary> handType == 1, the arm that holds the gun </summary>
    /// <summary> handType >= 2, knuckleblaster </summary>
    public static Texture HandTexture(Team team, byte handType = 0)
    {
        var s = (handType == 0) ? Settings.FeedColor : 
                (handType == 1) ? Settings.HandColor :
                                  Settings.KnuckleColor;

        if (handType == 0)
        {
            if (LobbyController.Offline && s == 0) return HandTextures[0];
            return s switch
            {
                0 => (team == Team.Blue) ? HandTextures[0] :
                     (team == Team.Red)  ? HandTextures[0] :
                                           HandTextures[1],
                1 => HandTextures[1],
                2 => HandTextures[4],
                _ => HandTextures[0]
            };
        }
        else if (handType == 1)
        {
            if (LobbyController.Offline && s == 0) return HandTextures[0];
            return s switch
            {
                0 => (team == Team.Blue) ? HandTextures[0] :
                     (team == Team.Red)  ? HandTextures[4] :
                                           HandTextures[1],
                1 => HandTextures[1],
                2 => HandTextures[4],
                _ => HandTextures[0]
            };
        }

        if (LobbyController.Offline && s == 0) return HandTextures[2];
        return s switch
        {
            0 => (team == Team.Blue) ? HandTextures[2] :
                 (team == Team.Red)  ? HandTextures[2] :
                                       HandTextures[3],
            1 => HandTextures[3],
            3 => HandTextures[5],
            _ => HandTextures[2]
        };
    }
}
