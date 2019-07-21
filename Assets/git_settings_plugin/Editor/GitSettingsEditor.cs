using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GitSettings.Editor
{
    public class GitSettingsEditor : EditorWindow
    {
        [MenuItem("Window/Git Settings")]
        private static void OpenWindow()
        {
            GetWindow<GitSettingsEditor>("Git Settings");
        }
        
        private void OnGUI()
        {
            var existsGit = ExistsGit();
            
            EditorGUILayout.LabelField("このプロジェクトのGit: ", existsGit ? "有効！" : "未作成...");
            EditorGUILayout.Space();
            
            using (new EditorGUI.DisabledScope(!existsGit))
            {
                DrawEditorSettingsGUI();
                DrawJetBrainGitKeepGUI();
            }
        }

        private bool ExistsGit()
        {
            var projectDir = Path.GetDirectoryName(Application.dataPath);
            var existsGit = Directory.Exists(projectDir + "/" + ".git");

            return existsGit;
        }

        private void DrawEditorSettingsGUI()
        {
            var isChangedSettings = 
                EditorSettings.externalVersionControl == ExternalVersionControl.Generic &&
                UnityEditor.EditorSettings.serializationMode == SerializationMode.ForceText;
            
            using (new EditorGUI.DisabledScope(isChangedSettings))
            {
                if (GUILayout.Button("Editor設定で.meta管理を有効化", GUILayout.Height(30f)))
                {
                    UnityEditor.EditorSettings.externalVersionControl = ExternalVersionControl.Generic; // "Visible Meta Files";
                    UnityEditor.EditorSettings.serializationMode = SerializationMode.ForceText;
                }
            }
        }

        private void DrawJetBrainGitKeepGUI()
        {
            
            var rootDirectory = "Plugins/Editor";
            var rootFullDirectory = Application.dataPath + "/" + rootDirectory;
            var fileName = ".gitkeep";

            var existFile = File.Exists(rootFullDirectory + "/" + fileName);

            using (new EditorGUI.DisabledScope(existFile))
            {
                if (GUILayout.Button("Jetbrainに.gitkeep作成", GUILayout.Height(30f)))
                {
                    //ディレクトリ作成
                    CreateDirectory(Path.Combine(rootFullDirectory));
                    //gitkeepファイル作成
                    CreateEmptyText(rootFullDirectory, fileName);
                
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private void CreateEmptyText(string directory, string fileName)
        {
            //gitkeepファイル作成
            using (var writer = File.CreateText(directory + "/" + fileName))
            {
                //空ファイルのためそのまま閉じる
                writer.Close();
            }
        }

        private void CreateDirectory(string relativeDir)
        {
            if (Directory.Exists(relativeDir))
            {
                return;
            }
            
            Directory.CreateDirectory(relativeDir);
        }
    }
}