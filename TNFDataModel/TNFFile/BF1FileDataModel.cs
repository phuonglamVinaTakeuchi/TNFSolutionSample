using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNFDataModel.Block;
using TNFDataModel.TNFDataAttribute;
using TNFDataModel.TNFModel;
using TNFDataModel.Utilities;

namespace TNFDataModel.TNFFile
{
    public class BF1FileDataModel : TNFFileContentModel
    {
        #region Property
        public string FilePath { get; private set; }
        /// <summary>
        /// Khoi thong tin cho Foundation
        /// </summary>
        [BlockContent("Foundation Block","@直接基礎データ", typeof(FoundationBlock), "@")]
        public BlockContentsBase FoundationBlock { get; set; }

        /// <summary>
        /// Khoi thong tin co ban, dinh nghia truc va cac thong tin co ban cua cong trinh
        /// </summary>
        [BlockContent("BasicInfo Block","@基本データ", typeof(BasicInfoBlock), "@")]
        public BlockContentsBase BasicBlock {  get; set;}

        /// <summary>
        /// Khoi thong tin cho cac doi tuong tren plan mat bang
        /// </summary>
        [BlockContent("Plan Block","@伏図データ", typeof(PlanBlock), "@")]
        public BlockContentsBase PlanBlock { get; set; }

        /// <summary>
        /// Dữ liệu vị trí trên Plan
        /// </summary>
        public List<LocationData> LocationData { get;private  set; }
        public List<TNFFoundationInstance> FoundationInstances { get; private set; }
        #endregion

        public BF1FileDataModel(string filePath,string[] sourceContent,string blockName) : base(sourceContent,blockName)
        {
            FilePath = filePath;
            InitLocationData();
        }
        private void InitLocationData()
        {
            LocationData = new List<LocationData>();
            FoundationInstances = new List<TNFFoundationInstance>();
            var glMatrix = ((PlanBlock)PlanBlock).GLMatrix.Matrixs;
            var xAxists = ((BasicInfoBlock)BasicBlock).XAxistName;
            var yAxist = ((BasicInfoBlock)BasicBlock).YAxistName.Reverse().ToArray();
            var foundationMatrixLocation = ((PlanBlock)PlanBlock).FoundationLocationMatrix.Matrixs;
            var hashiraGataMatrixX = ((PlanBlock)PlanBlock).HashiraGataDxMatrix.Matrixs;
            var hashiraGataMatrixY = ((PlanBlock)PlanBlock).HashiraGataDyMatrix.Matrixs;
            var foundationOffsetXMatrix = ((PlanBlock)PlanBlock).FoundationOfssetXMatrix.Matrixs;
            var foundationOffsetYMatrix = ((PlanBlock)PlanBlock).FoundationOfssetYMatrix.Matrixs;
            var hashiraGataLocationXMatrix = ((PlanBlock)PlanBlock).ColumnOffsetXMatrix.Matrixs;
            var hashiraGataLocationYMatrix = ((PlanBlock)PlanBlock).ColumnOffsetYMatrix.Matrixs;
            var foundationTypes = ((FoundationBlock)FoundationBlock).FoundationCollection.CollectionData;
            var xCount = xAxists.Count();
            var yCount = yAxist.Count();

            //var foundations = from fType in ((FoundationBlock)FoundationBlock).FoundationCollection.CollectionData
            //                  where !string.IsNullOrEmpty(fType.TypeMark)
            //                  select fType;

            for (var i = 0; i<xCount; i++)
            {
                for(var j =0; j < yCount; j++)
                {
                    var locationBuilder = new LocationBuilder();

                    var foundationIndex = foundationMatrixLocation[j, i];
                    //FoundationTypes foundationType = null;

                    var foundationType =  foundationTypes.FirstOrDefault(x => x.Index == foundationIndex);

                    var foundationOffsetX = foundationOffsetXMatrix!=null?foundationOffsetXMatrix[j, i]:"0";
                    var foundationOffsetY = foundationOffsetYMatrix !=null? foundationOffsetYMatrix[j, i]:"0";
                    var hashiraDimX = hashiraGataMatrixX != null ? hashiraGataMatrixX[j, i] : "0";
                    var hashiraDimY = hashiraGataMatrixY != null ? hashiraGataMatrixY[j, i] : "0";
                    var hashiraGataLocationX = hashiraGataLocationXMatrix!=null?hashiraGataLocationXMatrix[j,i] : "0";
                    var hashiraGataLocationY = hashiraGataLocationXMatrix!=null?hashiraGataLocationXMatrix[j,i] : "0";

                    var locationInfo = locationBuilder.SetupGridIntersection(xAxists[i], yAxist[j])
                                         .SetFoundationData(foundationType)
                                         .SetupOffsetDim(foundationOffsetX, foundationOffsetY)
                                         .SetHashiraGataDimension(hashiraDimX, hashiraDimY)
                                         .SetupHashiraGataColumnOffset(hashiraGataLocationX,hashiraGataLocationY)
                                         .Build();
                    if (locationInfo.FoundationType!=null && !string.IsNullOrEmpty(locationInfo.FoundationType.TypeMark) &&!string.IsNullOrEmpty(glMatrix[j,i]))
                    {
                        var foundationInstance = new TNFFoundationInstance();
                        foundationInstance.TypeMark = foundationType.TypeMark;
                        foundationInstance.GridIntersection = locationInfo.InterSectionName;
                        foundationInstance.FoundationOffsetX = locationInfo.GridOffsetX;
                        foundationInstance.FoundationOffsetY = locationInfo.GridOffsetY;
                        FoundationInstances.Add(foundationInstance);
                    }


                    LocationData.Add(locationInfo);


                }
            }

        }

    }
}
