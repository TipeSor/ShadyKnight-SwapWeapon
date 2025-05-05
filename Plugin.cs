using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace SwapWeapon
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<KeyCode> KeyConfig { get; private set; }
        public static KeyCode Key;
        public static BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

        public void Awake()
        {
            KeyConfig = Config.Bind("General", "Switch Key", KeyCode.Tab, "Key to switch active and backup weapons");
            Key = KeyConfig.Value;
        }

        public void Update()
        {
            if (Input.GetKeyDown(Key))
            {
                PlayerWeapons weapons = Game.player.weapons;

                FieldInfo currentField = typeof(PlayerWeapons).GetField("<currentWeapon>k__BackingField", flags);
                FieldInfo fallbackField = typeof(PlayerWeapons).GetField("<fallbackWeapon>k__BackingField", flags);

                if (currentField != null && fallbackField != null)
                {
                    int current = weapons.currentWeapon;
                    int fallback = weapons.fallbackWeapon;

                    currentField.SetValue(weapons, fallback);
                    fallbackField.SetValue(weapons, current);
                    weapons.Refresh();
                }
                else
                {
                    Debug.LogError("Reflection failed: can't find backing fields.");
                }


            }
        }
    }
}
