namespace CSharpLogic
{
    public enum RepresentationType
    {
        Explicit, Implicit, Parametric
    }

    public enum CoordinateSystemType
    {
        Cartesian, Polar
    }
 
    public enum ShapeType
    {
        Point = 0,
        Line = 1,
        LineSegment = 2,
        Circle = 3,
        PointLine = 4,
        TwoLines = 5,
        None = -1       
    }
}
