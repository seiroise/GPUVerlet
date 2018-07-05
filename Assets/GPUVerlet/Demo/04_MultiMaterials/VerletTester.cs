using Seiro.GPUVerlet.Core;
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
		public MaterialDictionary materialDictionary;

		private void Start()
		{
			
		}

		private void Update()
		{
			vSimulator.Simulate();
			vRenderer.Render();
		}

		RefStructure Build()
		{
			var s = new RefStructure();



			return s;
		}
	}
}