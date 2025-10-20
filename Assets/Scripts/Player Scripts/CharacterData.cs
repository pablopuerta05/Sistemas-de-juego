using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "2D Top-Down Rogue-like/Character Data")]
public class CharacterData : ScriptableObject
{
    [SerializeField] private Sprite icon;
    public Sprite Icon { get => icon; set => icon = value; }

    [SerializeField] private new string name;
    public string Name { get => name; set => name = value; }

    [SerializeField] private WeaponData startingWeapon;
    public WeaponData StartingWeapon { get => startingWeapon; set => startingWeapon = value; }

    [System.Serializable]
    public struct Stats
    {
        public float maxHealth, recovery, moveSpeed;
        public float might, projectileSpeed, magnet;

        public Stats(float maxHealth = 1000, float recovery = 0, float moveSpeed = 1f, float might = 1f, float projectileSpeed = 1f, float magnet = 30f)
        {
            this.maxHealth = maxHealth;
            this.recovery = recovery;
            this.moveSpeed = moveSpeed;
            this.might = might;
            this.projectileSpeed = projectileSpeed;
            this.magnet = magnet;
        }

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.recovery += s2.recovery;
            s1.moveSpeed += s2.moveSpeed;
            s1.might += s2.might;
            s1.projectileSpeed += s2.projectileSpeed;
            s1.magnet += s2.magnet;
            return s1;
        }
    }

    public Stats stats = new Stats(1000);
}
