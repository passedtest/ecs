using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ECS.UnityProxy;

[CustomEditor(typeof(ProxyComponent))]
public class ProxyComponentEditor : Editor
{
    SerializedProperty m_Componenets;
    Dictionary<string, Type> m_ComponenentTypes;
    Dictionary<string, bool> m_Foldouts;

    public void OnEnable()
    {
        m_Componenets = serializedObject.FindProperty("m_Components");

        m_Foldouts = new Dictionary<string, bool>();

        m_ComponenentTypes = new Dictionary<string, Type>();
        foreach (var typeHashCode in ProxyComponentUtility.GetComponenentTypeHashCodes())
        {
            if(ECS.Core.ComponentTypeUtility.TryGetType(typeHashCode, out var type))
                m_ComponenentTypes.Add(type.Name, type);
        }
    }

    public override void OnInspectorGUI()
    {
        var proxyCompopnenent = target as ProxyComponent;
        if (proxyCompopnenent == null)
            return;

        if (GUILayout.Button("Add componenent"))
        {
            var menu = new GenericMenu();
            foreach (var kvp in m_ComponenentTypes)
            {
                if(proxyCompopnenent.CanAddComponent(kvp.Value))
                    menu.AddItem(new GUIContent(kvp.Key), false, OnAddComponenentRequest, kvp.Key);
            }

            menu.ShowAsContext();
        }

        serializedObject.Update();
        for (var i = 0; i < m_Componenets.arraySize; i++)
        {
            var property = m_Componenets.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();
            {
                var managedReferenceFullTypename = property.managedReferenceFullTypename;

                var splitted = managedReferenceFullTypename.Split(' ');
                var typeFullName = splitted[splitted.Length - 1].Trim();

                if(ECS.Core.ComponentTypeUtility.TryGetType(typeFullName, out var type))
                {
                    var displayName = type.FullName;
                    if (!m_Foldouts.ContainsKey(managedReferenceFullTypename))
                        m_Foldouts.Add(managedReferenceFullTypename, true);

                    EditorGUILayout.BeginVertical();
                    {
                        m_Foldouts[managedReferenceFullTypename] = EditorGUILayout.Foldout(m_Foldouts[managedReferenceFullTypename], displayName, true, EditorStyles.foldoutHeader);

                        if (m_Foldouts[managedReferenceFullTypename])
                        {
                            foreach (SerializedProperty prop in property)
                            {
                                if (prop.depth > 2)
                                    continue;

                                EditorGUILayout.BeginHorizontal();
                                if (prop.propertyType != SerializedPropertyType.Quaternion)
                                    EditorGUILayout.PropertyField(prop, true);
                                else
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var eurelAngels = EditorGUILayout.Vector3Field(prop.displayName, prop.quaternionValue.eulerAngles);
                                    if (EditorGUI.EndChangeCheck())
                                        prop.quaternionValue = Quaternion.Euler(eurelAngels);

                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUI.BeginDisabledGroup(type.IsPersistentProxyComponent());
                    if (GUILayout.Button("Remove", GUILayout.MaxWidth(100)))
                        m_Componenets.DeleteArrayElementAtIndex(i);
                    EditorGUI.EndDisabledGroup();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
        serializedObject.ApplyModifiedProperties();
    }

    void OnAddComponenentRequest(object option)
    {
        try
        {
            var proxyCompopnenent = target as ProxyComponent;
            if (proxyCompopnenent == null)
                return;

            var componenentName = (string)option;
            proxyCompopnenent.TryAddComponenent(m_ComponenentTypes[componenentName]);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
