using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    WeaponData WeaponData;
    string[] weaponSubtypes;
    int selectedWeaponSubtype;

    private void OnEnable()
    {
        // Cache the weapon data value.
        WeaponData = (WeaponData)target;

        // retrieve all the weapon subtypes and cache it.
        System.Type baseType = typeof(Weapon);
        List<System.Type> subTypes = System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => baseType.IsAssignableFrom(p) && p != baseType).ToList();

        // Add a None option in front.
        List<string> subTypesString = subTypes.Select(t => t.Name).ToList();
        subTypesString.Insert(0, "None");
        weaponSubtypes = subTypesString.ToArray();

        // Ensure that we are using the correct weapon subtype.
        selectedWeaponSubtype = Math.Max(0, Array.IndexOf(weaponSubtypes, WeaponData.behaviour));
    }

    public override void OnInspectorGUI()
    {
        // Draw a dropdown in the inspector.
        selectedWeaponSubtype = EditorGUILayout.Popup("Behaviour", Math.Max(0, selectedWeaponSubtype), weaponSubtypes);

        if (selectedWeaponSubtype > 0)
        {
            // Updates the behaviour field.
            WeaponData.behaviour = weaponSubtypes[selectedWeaponSubtype].ToString();
            EditorUtility.SetDirty(WeaponData); // marks the object to save.
            DrawDefaultInspector(); // Draw the default inspector elements.
        }
    }
}
