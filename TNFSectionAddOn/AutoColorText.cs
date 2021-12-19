using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNFSectionAddOn
{
    public class AutoColorText
    {


            public  Color MAU1 = Color.FromColor(System.Drawing.Color.White);
            public  Color MAU2 = Color.FromColor(System.Drawing.Color.Red);
            public Color MAU3 = Color.FromColor(System.Drawing.Color.Yellow);


        [CommandMethod("AutoColor")]
        public void AutoColorForText()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            PromptSelectionOptions entityOption = new PromptSelectionOptions();
            var types = new TypedValue[1];
            var typeOption = new TypedValue((int)DxfCode.Start,"TEXT,MTEXT");


            //
            //types[0] = typeOption;
            types.SetValue(typeOption, 0);

            var filter = new SelectionFilter(types);
            //Type textType = typeof(MText);
            //entityOption.AddAllowedClass(textType,true);
            var result = acDoc.Editor.GetSelection(entityOption, filter);


            if (result.Status == PromptStatus.Cancel)
                return;
            var ents = result.Value;
            var entIds = ents.GetObjectIds();
            var strings = new Dictionary<string,Color>();
            var colors = new List<Color>() { MAU1, MAU2,MAU3 };
            

            var colorIndex = 0;

            using(var transaction = acDoc.Database.TransactionManager.StartTransaction())
            {
                foreach (var entId in entIds)
                {
                    var text = transaction.GetObject(entId, OpenMode.ForWrite) as DBText;
                    if (strings.ContainsKey(text.TextString))
                    {
                        strings.TryGetValue(text.TextString,out var color);
                        text.Color = color;
                    }
                    else
                    {
                        if (colorIndex <= colors.Count - 1)
                        {
                            var newColor = colors[colorIndex];
                            strings.Add(text.TextString, newColor);
                            colorIndex++;
                            text.Color = newColor;
                        }

                    }
                }
                transaction.Commit();
            }





        }
    }
}
