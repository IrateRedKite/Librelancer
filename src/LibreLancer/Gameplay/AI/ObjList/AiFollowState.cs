using System.Numerics;

namespace LibreLancer.AI.ObjList
{
    public class AiFollowState : AiObjListState
    {
        public string Target;
        public Vector3 Offset;

        public AiFollowState(string target, Vector3 offset)
        {
            Target = target;
        }
        
        public override void OnStart(GameObject obj, SNPCComponent ai)
        {
            var tgtObject = obj.World.GetObject(Target);
            ai.EnterFormation(tgtObject, Offset);
            ai.SetState(Next);
        }

        public override void Update(GameObject obj, SNPCComponent ai, double dt) { }
    }
}