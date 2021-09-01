namespace Botli.Modules.Maths
{
    public struct FunctionInfo
    {
        public Function Function { get; private set; }

        public string Info { get; private set; }

        public string[] ParameterInfo { get; private set; }

        public FunctionInfo(Function function, string info, params string[] parameterInfo)
        {
            Function = function;
            Info = info;
            ParameterInfo = parameterInfo;
        }

        public static implicit operator FunctionInfo((Function, string) valueTuple) 
            => new FunctionInfo(valueTuple.Item1, valueTuple.Item2);
    }
}
