public interface ISpecialAction
{
    void Execute(CharacterEntityController character, CharacterEntityController targetObject);

    void SetSPCost(CharacterEntityController character);

    void SetSpecialText(CharacterEntityController character);
}

