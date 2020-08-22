using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillID
{
    SKILL_STRENGTH,
    SKILL_CRAFTING,
    SKILL_BOOKKEEPING,
    SKILL_COOKING,
    SKILL_MEDICINE
}
public abstract class Skill : MonoBehaviour
{
    public static SkillID skillID;

    public int skillLevel;
    public int experience;
    public static int skillLevelCap = 10;

    private int[] xpRequirements = { 5, 10, 20, 40, 80, 160, 320, 640, 1280, 2560 };

    public int ExperienceForNextLevel(int currLevel)    // Returns the total accumulated experience required to level up;
    {
        if(currLevel > skillLevelCap)
        {
            return int.MaxValue;
        }
        return xpRequirements[currLevel];
    }

    public bool GainExperience(int newXP)
    {
        experience += newXP;
        return UpdateLevel();
    }

    public bool UpdateLevel()
    {
        if(experience >= ExperienceForNextLevel(skillLevel))
        {
            skillLevel++;
            UpdateLevel();
            return true;
        }
        return false;
    }

    public abstract float SkillMultipler();


}
