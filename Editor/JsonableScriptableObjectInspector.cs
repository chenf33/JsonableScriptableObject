using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;

namespace am
{

[CustomEditor(typeof(JsonableScriptableObject))]
public class JsonableScriptableObjectInspector : Editor
{
    protected JsonableScriptableObjectConfig m_config;
    
    protected virtual void OnEnable()
    {
	//var so = target as JsonableScriptableObject;
	var guids = UnityEditor.AssetDatabase.FindAssets("t:JsonableScriptableObjectConfig");
	if (guids.Length == 0) {
	    //throw new System.IO.FileNotFoundException ("JsonableScriptableObjectConfig does not found");
	    var dir = "Assets/AmPlugins/Settings/";
	    if(!Directory.Exists(dir)){ Directory.CreateDirectory(dir); }
	    var asset = CreateInstance<JsonableScriptableObjectConfig>();
	    AssetDatabase.CreateAsset
		(asset, "Assets/AmPlugins/Settings/JsonableScriptableObjectConfig.asset");
	    AssetDatabase.Refresh();
	    guids = UnityEditor.AssetDatabase.FindAssets("t:JsonableScriptableObjectConfig");
	}
	var path = AssetDatabase.GUIDToAssetPath(guids[0]);
	m_config = AssetDatabase.LoadAssetAtPath<JsonableScriptableObjectConfig>(path);
    }

    
    public override void OnInspectorGUI()
    {
	var jso = target as JsonableScriptableObject;
	DrawConvertMenu(jso);
	//DrawDefaultInspector();
    }

    /*=================================================================================================*/

    protected string GetJsonFile(){
	string json = "";
	var jso  = target as JsonableScriptableObject;
	var path = m_config.jsonDirPath + "/" + jso.name + ".json";
	if(! File.Exists(path)){ Debug.Log(path + " Not Found !"); }
	else {	    
	    using(var sr = new StreamReader(path, Encoding.GetEncoding("utf-8"))){ json = sr.ReadToEnd(); }
	}
	return json;
    }
    protected void PutJsonFile(string json){
	var jso  = target as JsonableScriptableObject;
	var path = m_config.jsonDirPath + "/" + jso.name + ".json";
	using(var sw = new StreamWriter(path, false, Encoding.GetEncoding("utf-8"))){ sw.Write(json); }
    }
    protected virtual void ImportFromJson(){}    
    protected virtual void ExportToJson(){}

    protected virtual void DrawConvertMenu(JsonableScriptableObject jso){
	DrawSimpleLabelField("JsonDirPath : ", m_config.jsonDirPath);
	DrawSimpleLabelField("JsonName : ",    jso.name);
	EditorGUILayout.BeginHorizontal();
	{
	    if(GUILayout.Button("Import", EditorStyles.miniButton)){
		ImportFromJson();
		EditorUtility.SetDirty(jso);
		EditorUtility.DisplayDialog("JsonImport", "Complete", "OK");	
	    }
	    if(GUILayout.Button("Export", EditorStyles.miniButton)){
		ExportToJson();
		//EditorUtility.SetDirty(jso);
		EditorUtility.DisplayDialog("JsonExport", "Complete", "OK");			
	    }
	}
	EditorGUILayout.EndHorizontal();
	GUILayout.Space(5);	
	GUILayout.Box(GUIContent.none, HrStyle.EditorLine, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
	GUILayout.Space(5);	
    }

    protected void DrawSimpleLabelField(string label, string value,
					float defaultLabelWidth = 80f)
    {
	EditorGUILayout.BeginHorizontal();
	{
	    EditorGUILayout.LabelField(label, GUILayout.Width(defaultLabelWidth));
	    if(value != ""){
		EditorGUILayout.LabelField(value);
	    }
	}
	EditorGUILayout.EndHorizontal();	
    }
    
    protected void DrawSimpleTextField(JsonableScriptableObject jso, string label, ref string value,
				       float defaultLabelWidth = 80f)
    {
	EditorGUILayout.BeginHorizontal();
	{
	    EditorGUILayout.LabelField(label, GUILayout.Width(defaultLabelWidth));
	    var input = EditorGUILayout.TextField(value);
	    if(input != value){
		Undo.RegisterCompleteObjectUndo(jso, label + " Change");
		value = input;
		EditorUtility.SetDirty(jso);
	    }
	}
	EditorGUILayout.EndHorizontal();	
    }
    
    protected void DrawSimpleIntField(JsonableScriptableObject jso, string label, ref int value,
				       float defaultLabelWidth = 80f)
    {
	EditorGUILayout.BeginHorizontal();
	{
	    EditorGUILayout.LabelField(label, GUILayout.Width(defaultLabelWidth));
	    var input = EditorGUILayout.IntField(value);
	    if(input != value){
		Undo.RegisterCompleteObjectUndo(jso, label + " Change");
		value = input;
		EditorUtility.SetDirty(jso);
	    }
	}
	EditorGUILayout.EndHorizontal();	
    }
    
}
}    
