public interface IMinigame
{
    void StartSelectedMinigameServerRpc();

    bool SetPlayerKnockbackBool { get; set; }

    void SetPlayerKnockback(bool shouldSet);
}