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
                            var isTransformComponenent = type.Equals(typeof(ECS.TransformComponent));
                            if (isTransformComponenent)
                            {
                                property.managedReferenceValue = ECS.TransformComponent.Create(proxyCompopnenent.transform);
                                EditorGUILayout.HelpBox("Chanage object's transofrm in order to change this componenent", MessageType.Info);
                            }

                            var isGameObjectComponenent = type.Equals(typeof(ECS.GameObjectProxyComponenet));
                            if (isGameObjectComponenent)
                            {
                                property.managedReferenceValue = ECS.GameObjectProxyComponenet.Create(proxyCompopnenent.gameObject);
                                EditorGUILayout.HelpBox("Chanage object in order to change this componenent", MessageType.Info);
                            }

                            EditorGUI.BeginDisabledGroup(isTransformComponenent || isGameObjectComponenent);
                            {
                                foreach (SerializedProperty prop in property)
                                {
                                    if (prop.depth > 2)
                                        continue;

                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.PropertyField(prop, true);
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                            EditorGUI.EndDisabledGroup();

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
