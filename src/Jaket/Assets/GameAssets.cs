namespace Jaket.Assets;

using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary> Class that works with the assets of the game. </summary>
public class GameAssets
{
    #region content

    /// <summary> List of items that mustn't be synchronized, because they are not items at all. </summary>
    public static readonly string[] ItemExceptions =
    { "Minotaur", "Tram (3)", "BombTrigger", "BombStationTramTeleporterKey", "Checker" };

    /// <summary> List of internal names of all enemies. </summary>
    public static readonly string[] Enemies =
    {
        "Zombie", "Projectile Zombie", "Super Projectile Zombie", "ShotgunHusk", "MinosBoss", "Stalker", "Sisyphus", "Ferryman",
        "SwordsMachineNonboss", "Drone", "Streetcleaner", "Mindflayer", "V2", "V2 Green Arm Variant", "Turret", "Gutterman",
        "Guttertank", "Spider", "StatueEnemy", "Mass", "Idol", "Mannequin", "Minotaur", "Virtue",
        "Gabriel", "Gabriel 2nd Variant", "Wicked", "Flesh Prison", "DroneFlesh", "Flesh Prison 2", "DroneFleshCamera Variant", "DroneSkull Variant",
        "MinosPrime", "SisyphusPrime", "Cancerous Rodent", "Very Cancerous Rodent", "Mandalore", "Big Johninator", "Puppet"
    };

    /// <summary> List of internal names of all items. </summary>
    public static readonly string[] Items =
    { ".Apple Bait", ".Maurice Bait", "SkullBlue", "SkullRed", "Soap", "Torch", "Florp Throwable" };

    /// <summary> List of internal names of all baits. </summary>
    public static readonly string[] Baits =
    {};

    /// <summary> List of internal names of all fishes. </summary>
    public static readonly string[] Fishes =
    {
        // "Funny Fish!!!", "pitr fish", "Trout", "Amid Efil Fish", "Dusk Chomper",
        // "Bomb Fish", "Gib Eyeball Fish", "IronLungFish", "Dope Fish", "Fish Stick",
        // "Cooked Fish", "Shark Fish", "Burnt Stuff"
    };

    /// <summary> List of internal names of all dev plushies. </summary>
    public static readonly string[] Plushies =
    {
        "Jacob", "Mako", "HEALTH - Jake", "Dalia", "Jericho", "Meganeko", "Tucker", "BigRock", "Dawg", "Sam",
        "Cameron", "Gianni", "Salad", "Mandy", "Joy", "Weyte", "Heckteck", "Hakita", "Lenval", ". (CabalCrow) Variant",
        "Quetzal", "HEALTH - John", "PITR", "HEALTH - BJ", "Francis", "Vvizard", "Lucas", "Scott", "KGC", "."
    };

    /// <summary> List of readable names of all dev plushies. </summary>
    public static readonly string[] PlushiesButReadable =
    {
        "Jacob", "Maximilian", "Jake", "Dalia", "Jericho", "Meganeko", "Tucker", "BigRock", "Victoria", "Samuel",
        "Cameron", "Gianni", "Salad", "Mandy", "Joy", "Weyte", "Heckteck", "Hakita", "Lenval", "CabalCrow",
        "Quetzal", "John", "Pitr", "BJ", "Francis", "Vvizard", "Lucas", "Scott", "KGC", "V1"
    };

    #endregion
    #region tools

    private static GameObject Prefab(string name) => AssetHelper.LoadPrefab($"Assets/Prefabs/{name}.prefab");

    private static void Material(string name, Action<Material> cons) => Addressables.LoadAssetAsync<Material>($"Assets/Models/{name}.mat").Task.ContinueWith(t => cons(t.Result));

    private static void Sound(string name, Action<AudioClip> cons) => Addressables.LoadAssetAsync<AudioClip>($"Assets/Sounds/{name}.ogg").Task.ContinueWith(t => cons(t.Result));

    #endregion
    #region loading

    public static GameObject Enemy(string name) => Prefab($"Enemies/{name}");

    public static GameObject Item(string name) => Prefab($"Items/{name}");

    public static GameObject Bait(string name) => Prefab($"Fishing/{name}");

    public static GameObject Fish(string name) => Prefab($"Fishing/Fishes/{name}");

    public static GameObject Plushie(string name) => Prefab($"Items/DevPlushies/DevPlushie{(name.StartsWith(".") ? name.Substring(1) : $" ({name})")}");

    /// <summary> Loads the torch prefab. </summary>
    public static GameObject Torch() => Prefab("Levels/Interactive/Altar (Torch) Variant");

    /// <summary> Loads the blast explosion prefab. </summary>
    public static GameObject Blast() => Prefab("Attacks and Projectiles/Explosions/Explosion Wave");

    /// <summary> Loads the shotgun pickup prefab. </summary>
    public static GameObject Shotgun() => Prefab("Weapons/Pickups/ShotgunPickUp");

    /// <summary> Loads the squeaky toy sound prefab. </summary>
    public static GameObject Squeaky() => AssetHelper.LoadPrefab("Assets/Particles/SoundBubbles/SqueakyToy.prefab");

    /// <summary> Loads the fish pickup prefab. </summary>
    public static GameObject FishTemplate() => Prefab("Fishing/Fish Pickup Template");

    /// <summary> Loads a swordsmachine material by name. </summary>
    public static void SwordsMaterial(string name, Renderer output) => Material($"Enemies/SwordsMachine/{name}", mat => output.material = mat);

    /// <summary> Loads an insurrectionist material by name. </summary>
    public static void SisyMaterial(string name, Renderer[] output) => Material($"Enemies/Sisyphus/{name}", mat => output[0].material = output[1].material = mat);

    /// <summary> Loads a Gabriel voice line by name. </summary>
    public static void GabLine(string name, Action<AudioClip> output) => Sound($"Voices/Gabriel/{name}", output);

    #endregion
}
