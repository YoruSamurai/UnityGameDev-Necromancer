

using LDtkUnity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LDtkUnity
{

    public class ExamplePreprocessor : LDtkPreprocessor
    {
        protected override void OnPreprocessLevel(Level level, LdtkJson projectJson, string projectName)
        {
            Debug.Log($"Pre process LDtk level: {level}");

        }

        protected override void OnPreprocessProject(LdtkJson projectJson, string projectName)
        {
            Debug.Log($"Pre process LDtk project: {projectName}");

        }
    }
}

