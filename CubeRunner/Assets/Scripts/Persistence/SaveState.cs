namespace Assets.Scripts.Persistence
{
    public class SaveState
    {
        public int HighScore = 0;

        public void SetState(int highscore)
        {
            HighScore = highscore;
        }
    }
}