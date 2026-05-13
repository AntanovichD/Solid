// Заглушки публичных типов SolidWorks Interop API. Используются только в CI,
// чтобы код собирался без установленного SolidWorks. Сигнатуры воспроизводят
// только те методы и свойства, на которые ссылается код в Lab4/Lab5.

#if SLDWORKS
namespace SolidWorks.Interop.sldworks
{
    public interface ISldWorks
    {
        bool Visible { get; set; }
        bool UserControl { get; set; }
        string GetUserPreferenceStringValue(int id);
        object NewDocument(string template, int paper, double width, double height);
    }

    public interface IModelDoc2
    {
        ISketchManager SketchManager { get; }
        IFeatureManager FeatureManager { get; }
        IModelDocExtension Extension { get; }
        void ClearSelection2(bool global);
        void EditRebuild3();
        void ViewZoomtofit2();
        object AddDimension2(double x, double y, double z);
        object AddRadialDimension2(double x, double y, double z);
        object AddDiameterDimension2(double x, double y, double z);
    }

    public interface IModelDocExtension
    {
        bool SelectByID2(string name, string type, double x, double y, double z,
                         bool append, int mark, object cb, int opts);
        bool SelectAll();
        int DeleteSelection2(int options);
        void SaveAs(string fileName, int version, int options,
                    object exportData, ref int errors, ref int warnings);
    }

    public interface ISketchManager
    {
        object InsertSketch(bool finishSketch);
        ISketchSegment CreateLine(double x1, double y1, double z1,
                                   double x2, double y2, double z2);
        ISketchSegment CreateCenterLine(double x1, double y1, double z1,
                                         double x2, double y2, double z2);
        ISketchSegment CreateCircleByRadius(double cx, double cy, double cz, double r);
        ISketchSegment CreateArc(double cx, double cy, double cz,
                                  double sx, double sy, double sz,
                                  double ex, double ey, double ez,
                                  short direction);
        ISketchPoint CreatePoint(double x, double y, double z);
    }

    public interface ISketchSegment
    {
        bool ConstructionGeometry { get; set; }
    }

    public interface ISketchPoint { }

    public interface IFeatureManager
    {
        IFeature FeatureExtrusion3(bool Sd, bool Flip, bool Dir,
            int T1, int T2, double D1, double D2,
            bool Dchk1, bool Dchk2, bool Ddir1, bool Ddir2,
            double Dang1, double Dang2,
            bool OffsetReverse1, bool OffsetReverse2,
            bool TranslateSurface1, bool TranslateSurface2,
            bool Merge, bool UseFeatScope, bool UseAutoSelect,
            int T0, double StartOffset, bool FlipStartOffset);

        IFeature FeatureCut4(bool Sd, bool Flip, bool Dir,
            int T1, int T2, double D1, double D2,
            bool Dchk1, bool Dchk2, bool Ddir1, bool Ddir2,
            double Dang1, double Dang2,
            bool OffsetReverse1, bool OffsetReverse2,
            bool TranslateSurface1, bool TranslateSurface2,
            bool NormalCut, bool UseFeatScope, bool UseAutoSelect,
            bool AssemblyFeatureScope, bool AutoSelectComponents,
            bool PropagateFeatureToParts,
            int T0, double StartOffset, bool FlipStartOffset,
            bool OptimizeGeometry);
    }

    public interface IFeature
    {
        object GetFaces();
    }

    public interface IEntity
    {
        bool Select4(bool append, object data);
    }

    public interface IDisplayDimension
    {
        object GetDimension2(int index);
    }

    public interface IDimension
    {
        int SetSystemValue3(double value, int configOpt, object configNames);
    }
}
#endif

#if SWCONST
namespace SolidWorks.Interop.swconst
{
    public enum swUserPreferenceStringValue_e
    {
        swDefaultTemplatePart = 9
    }

    public enum swSaveAsVersion_e
    {
        swSaveAsCurrentVersion = 0
    }

    public enum swSaveAsOptions_e
    {
        swSaveAsOptions_Silent = 1
    }

    public enum swDeleteSelectionOptions_e
    {
        swDelete_Absorbed = 1,
        swDelete_Advanced = 2,
        swDelete_Children = 4
    }

    public enum swInConfigurationOpts_e
    {
        swThisConfiguration = 1
    }

    public enum swEndConditions_e
    {
        swEndCondBlind = 0,
        swEndCondThroughAll = 1,
        swEndCondThroughNext = 2,
        swEndCondUpToVertex = 3,
        swEndCondUpToSurface = 4,
        swEndCondOffsetFromSurface = 5,
        swEndCondUpToBody = 6,
        swEndCondMidPlane = 7
    }

    public enum swStartConditions_e
    {
        swStartSketchPlane = 0,
        swStartSurface = 1,
        swStartVertex = 2,
        swStartOffset = 3
    }
}
#endif

#if SWCOMMANDS
namespace SolidWorks.Interop.swcommands
{
    public enum swCommands_e
    {
        swCommands_DefaultBlank = 0
    }
}
#endif
