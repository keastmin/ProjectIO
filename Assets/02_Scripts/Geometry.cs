using UnityEngine;

public static class Geometry
{
    const float EPS = 1e-7f;

    private static float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

    public static bool SegmentIntersection(Vector2 A, Vector2 B, Vector2 C, Vector2 D, bool includeEndpoints, out Vector2 intersection)
    {
        intersection = default;

        Vector2 r = B - A;
        Vector2 s = D - C;
        float rxs = Cross(r, s);
        Vector2 AC = C - A;
        float ACxr = Cross(AC, r);

        // 평행
        if (Mathf.Abs(rxs) < EPS)
        {
            // 공선 여부
            if (Mathf.Abs(ACxr) < EPS)
            {
                // 매개변수로 투영해 구간 겹침 확인
                float rr = Vector2.Dot(r, r);
                if (rr < EPS) // A==B(퇴화)
                {
                    // 점 A가 CD 위에 있나?
                    if (PointOnSegment(A, C, D))
                    {
                        intersection = A;
                        return true;
                    }
                    return false;
                }

                float t0 = Vector2.Dot(AC, r) / rr;
                float t1 = t0 + Vector2.Dot(s, r) / rr;
                float tmin = Mathf.Min(t0, t1);
                float tmax = Mathf.Max(t0, t1);

                if (tmax <= 0f || tmin >= 1f) return false; // 안 겹침

                // 겹침: 단일 교점을 원하면 경계점 중 하나를 반환
                float t = Mathf.Clamp01((Mathf.Abs(tmin - 1f) < Mathf.Abs(tmax) ? 1f : 0f));
                intersection = A + t * r;
                return true;
            }
            // 평행하지만 공선 아님
            return false;
        }

        // 비평행: t, u 계산
        float t_ = Cross(AC, s) / rxs;
        float u_ = Cross(AC, r) / rxs;

        if(includeEndpoints && t_ >= -EPS && t_ <= 1f + EPS && u_ >= -EPS && u_ <= 1f + EPS)
        {
            // 구간 안: 교점, 끝점 인식
            intersection = A + t_ * r;
            return true;
        }
        return false;
    }

    public static bool PointOnSegment(Vector2 P, Vector2 A, Vector2 B)
    {
        // 공선 확인
        if (Mathf.Abs(Cross(B - A, P - A)) > EPS) return false;
        // 바운딩 박스
        return Mathf.Min(A.x, B.x) - EPS <= P.x && P.x <= Mathf.Max(A.x, B.x) + EPS &&
               Mathf.Min(A.y, B.y) - EPS <= P.y && P.y <= Mathf.Max(A.y, B.y) + EPS;
    }
}