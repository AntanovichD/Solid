namespace SolidWorksApi
{
    /// <summary>
    /// SolidWorks API оперирует значениями в метрах. Все размеры в коде задаются в
    /// миллиметрах — этот класс централизованно делает преобразование.
    /// </summary>
    public static class Mm
    {
        public const double InMeter = 1000.0;

        public static double ToMeters(double mm) => mm / InMeter;

        public static double ToMm(double meters) => meters * InMeter;
    }
}
