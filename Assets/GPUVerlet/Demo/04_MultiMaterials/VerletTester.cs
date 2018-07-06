using Seiro.GPUVerlet.Core.Compilers;
using Seiro.GPUVerlet.Core.Controller;
using Seiro.GPUVerlet.Core.RefDatas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Seiro.GPUVerlet.Demo
{

	/// <summary>
	/// MultiMaterialのテスター
	/// </summary>
	public class VerletTester : MonoBehaviour
	{

		public bool autoBuildAndAssign = true;
		public VerletModel model;
		public MaterialDictionary materialDict;
		public Core.Architect.BaseArchitect architect;

		private void Start()
		{
			if (autoBuildAndAssign)
			{
				BuildAndAssign();
			}
		}

		void BuildAndAssign()
		{
			var normal = architect.Build(transform.localToWorldMatrix);
			var compiled = StructureCompiler.Compile(normal, materialDict);
			model.SetStructure(compiled);
		}

#if UNITY_EDITOR

		[CanEditMultipleObjects]
		[CustomEditor(typeof(VerletTester))]
		class InternalEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				DrawMultiGUI();
			}

			void DrawMultiGUI()
			{
				var selves = new VerletTester[targets.Length];
				for (var i = 0; i < targets.Length; ++i)
				{
					selves[i] = targets[i] as VerletTester;
				}

				if (GUILayout.Button("Build and Assign"))
				{
					for (var i = 0; i < selves.Length; ++i)
					{
						selves[i].BuildAndAssign();
					}
				}
			}
		}
#endif
	}
}