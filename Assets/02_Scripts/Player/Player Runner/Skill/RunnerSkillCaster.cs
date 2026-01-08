using UnityEngine;

public class RunnerSkillCaster
{
    public void Cast(RunnerSkillType skillType, object parameters = null)
    {
        switch (skillType)
        {
            case RunnerSkillType.Tumble:
                Tumble(parameters);
                break;
            case RunnerSkillType.OutOfBody:
                OutOfBody(parameters);
                break;
            case RunnerSkillType.Swiftness:
                Swiftness(parameters);
                break;
            default:
                Debug.Log("알 수 없는 스킬 타입");
                break;
        }
    }

    private void Tumble(object parameters)
    {
        var playerRunner = parameters as PlayerRunner;
        if (playerRunner == null)
        {
            Debug.LogError("Tumble 스킬 사용 시 PlayerRunner 파라미터가 필요합니다.");
            return;
        }

        // 캐릭터 애니메이션 재생
        playerRunner.StartTumble(); // 동작 실행
        playerRunner.StartInvincibility(1.0f); // 1초간 무적 상태 with Task
        Debug.Log("Tumble 스킬 사용 - 1초간 무적 상태");
    }

    private void OutOfBody(object parameters)
    {
        var playerRunner = parameters as PlayerRunner;
        if (playerRunner == null)
        {
            Debug.LogError("OutOfBody 스킬 사용 시 PlayerRunner 파라미터가 필요합니다.");
            return;
        }

        playerRunner.StartOutOfBody(); // 동작 실행 - 영혼 생성해서 앞으로 진행
    }

    private void Swiftness(object parameters)
    {
        var playerRunner = parameters as PlayerRunner;
        if (playerRunner == null)
        {
            Debug.LogError("Swiftness 스킬 사용 시 PlayerRunner 파라미터가 필요합니다.");
            return;
        }

        playerRunner.StartSwiftness(); // 동작 실행 - 무기 공격 속도, 장전 속도 대폭 증가, 기력 소모 없는 3회 슬라이드 획득
    }
}