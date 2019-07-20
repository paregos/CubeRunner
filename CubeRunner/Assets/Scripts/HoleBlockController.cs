namespace Assets.Scripts
{
    public class HoleBlockController : BlockController
    {
        public override bool IsHazard()
        {
            return true;
        }
    }
}