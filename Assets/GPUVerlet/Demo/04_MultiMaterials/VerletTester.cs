
using Seiro.GPUVerlet.Core.Compiler;
using Seiro.GPUVerlet.Core.Controller;
using Seiro.GPUVerlet.Core.RefDatas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.GPUVerlet.Demo
{

	/// <summary>
	/// MultiMaterialのテスター
	/// </summary>
	public class VerletTester : MonoBehaviour
	{
		public VerletSimulator vSimulator;
		public VerletRenderer vRenderer;
		public MaterialDictionary materialDict;
		public Core.Architect.BaseArchitect architect;

		private void Start()
		{
			var normal = architect.Build();
			var compiled = StructureCompiler.Compile(normal, materialDict);
			vSimulator.SetStructure(compiled);
			vRenderer.SetStructure(compiled);
		}

		private void Update()
		{
			vSimulator.Simulate();
			vRenderer.Render();
		}
	}
}