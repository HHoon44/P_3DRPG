using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace ProjectChan.Editor
{
    /// <summary>
    /// => 엑셀을 임포트 할 때 실행할 메서드를 지닌 클래스
    /// => AssetPostprocessor : 에셋이 변경될 때마다 콜백함수를 받을 수 있게해줌
    /// </summary>
    public class UnityChanRPGAssetPostProcessor : AssetPostprocessor
    {
        /// <summary>
        /// => 에셋이 임포트될 때 실행할 메서드
        /// </summary>
        /// <param name="importedAssets"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            StaticDataImporter.Import(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
        }
    }
}
