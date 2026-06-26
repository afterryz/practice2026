namespace task04
{
    public class Fighter : ISpaceship
    {
        public int Speed => 100;
        public int FirePower => 50;
        public int Coordinate { get; set; } = 0;
        public int Angle { get; set; } = 0;
        public int Shoot { get; set; } = 0;
        public void MoveForward()
        {
            Coordinate += Speed;
        }
        public void Rotate(int angle)
        {
            Angle = (Angle + angle) % 360;
        }
        public void Fire()
        {
            Shoot++;
        }
    }
}
