using UnityEngine;

public class PlayerBuilderReferenceBinder
{
    // 빌더 UI 참조를 플레이어 빌더에게 전달하는 함수
    public bool BuilderUIBind(PlayerBuilder builder, PlayerBuilderUI builderUI)
    {
        if (builder == null)
        {
            Debug.Log("빌더가 없습니다.");
            return false;
        }
        if(builderUI == null)
        {
            Debug.Log("빌더 UI가 없습니다.");
            return false;
        }

        // UI 참조 전달
        builder.PlayerBuilderReferenceInjection(builderUI);
        return true;
    }
}
