
public class ExperienceGem : Pickup
{
    public int experienceGranted;

    public override void Collect()
    {
        if (hasBeenCollected)
        {
            return;
        }
        else
        {
            base.Collect();
        }

        PlayerExperience player = FindAnyObjectByType<PlayerExperience>();
        player.IncreaseExperience(experienceGranted);
    }
}
