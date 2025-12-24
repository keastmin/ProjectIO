using System.Collections.Generic;
using UnityEngine;

public static class DragFrustumPicker
{
    public static List<Collider> CollectInDragFrustum(
        Rect screenRectPixels, Camera cam, LayerMask mask,
        float nearZ, float farZ,
        QueryTriggerInteraction qti = QueryTriggerInteraction.Collide
        )
    {
        // === 디버그 로그 추가 ===
        Debug.Log($"[DragFrustum] Screen Rect: {screenRectPixels}, Near: {nearZ}, Far: {farZ}");

        Vector3 Near(float x, float y) => cam.ScreenToWorldPoint(new Vector3(x, y, nearZ));
        Vector3 Far(float x, float y) => cam.ScreenToWorldPoint(new Vector3(x, y, farZ));

        float x0 = screenRectPixels.xMin, x1 = screenRectPixels.xMax;
        float y0 = screenRectPixels.yMin, y1 = screenRectPixels.yMax;

        var nTL = Near(x0, y1); var nTR = Near(x1, y1);
        var nBR = Near(x1, y0); var nBL = Near(x0, y0);
        var fTL = Far(x0, y1); var fTR = Far(x1, y1);
        var fBR = Far(x1, y0); var fBL = Far(x0, y0);

        // === 좌표 디버그 ===
        Debug.Log($"[DragFrustum] Near TL: {nTL}, Far TL: {fTL}");
        Debug.Log($"[DragFrustum] Cam Pos: {cam.transform.position}");

        var aabb = new Bounds(nTL, Vector3.zero);
        void Enc(Vector3 p) { aabb.Encapsulate(p); }
        Enc(nTR); Enc(nBR); Enc(nBL); Enc(fTL); Enc(fTR); Enc(fBR); Enc(fBL);

        Debug.Log($"[DragFrustum] AABB Center: {aabb.center}, Size: {aabb.size}");

        var camPos = cam.transform.position;

        // === 개선된 Plane 생성 로직 ===
        Plane MakePlane(Vector3 a, Vector3 b, Vector3 c)
        {
            var p = new Plane(a, b, c);

            // 카메라가 평면의 양의 방향(normal 방향)에 있어야 함
            // 평면의 normal이 카메라를 향하도록 (내부를 향하도록)
            float distToCam = p.GetDistanceToPoint(camPos);

            // 카메라가 음의 방향에 있으면 뒤집기
            if (distToCam < 0)
            {
                p.Flip();
            }

            return p;
        }

        // === 수정된 평면 생성 (정점 순서 확인) ===
        Plane[] planes = new Plane[6];

        // Near plane (카메라를 향함)
        planes[0] = MakePlane(nBL, nBR, nTR);

        // Far plane (카메라 반대 방향)
        planes[1] = MakePlane(fBR, fBL, fTL);

        // Left plane
        planes[2] = MakePlane(fBL, nBL, nTL);

        // Right plane
        planes[3] = MakePlane(nBR, fBR, fTR);

        // Top plane
        planes[4] = MakePlane(nTL, fTL, fTR);

        // Bottom plane
        planes[5] = MakePlane(fBL, nBL, nBR);

        // === 평면 방향 디버그 ===
        for (int i = 0; i < planes.Length; i++)
        {
            Debug.Log($"[DragFrustum] Plane {i}: Normal={planes[i].normal}, Distance={planes[i].distance}");
        }

        // 색상
        Color cNear = new Color(0f, 1f, 1f, 1f);   // cyan
        Color cFar = new Color(0f, 0.5f, 1f, 1f); // blue
        Color cEdge = new Color(0f, 1f, 0f, 1f);   // green
        Color cAabb = new Color(1f, 1f, 0f, 1f);   // yellow
        Color cNorm = new Color(1f, 0f, 1f, 1f);   // magenta

        void DL(Vector3 a, Vector3 b, Color c) => Debug.DrawLine(a, b, c, 2f, true); // 지속 시간 증가

        // near/far 사각형
        DL(nTL, nTR, cNear); DL(nTR, nBR, cNear); DL(nBR, nBL, cNear); DL(nBL, nTL, cNear);
        DL(fTL, fTR, cFar); DL(fTR, fBR, cFar); DL(fBR, fBL, cFar); DL(fBL, fTL, cFar);

        // 모서리 연결(near↔far)
        DL(nTL, fTL, cEdge); DL(nTR, fTR, cEdge); DL(nBR, fBR, cEdge); DL(nBL, fBL, cEdge);

        // (옵션) 평면 노멀 방향
        Vector3 C(params Vector3[] ps) { var s = Vector3.zero; for (int i = 0; i < ps.Length; i++) s += ps[i]; return s / ps.Length; }
        float normLen = Mathf.Max(0.5f, (farZ - nearZ) * 0.15f); // 노멀 길이 증가
        var centers = new (Vector3 center, Vector3 normal)[]
        {
            (C(nTL,nTR,nBR,nBL), planes[0].normal), // near
            (C(fTL,fTR,fBR,fBL), planes[1].normal), // far
            (C(nBL,nTL,fTL,fBL), planes[2].normal), // left
            (C(nTR,nBR,fBR,fTR), planes[3].normal), // right
            (C(nTL,fTL,fTR,nTR), planes[4].normal), // top
            (C(nBR,nBL,fBL,fBR), planes[5].normal), // bottom
        };
        foreach (var (center, normal) in centers)
            Debug.DrawLine(center, center + normal * normLen, cNorm, 2f, true);

        // (옵션) AABB 와이어
        var c = aabb.center; var e = aabb.extents;
        Vector3 V(float sx, float sy, float sz) => c + new Vector3(e.x * sx, e.y * sy, e.z * sz);
        var v000 = V(-1, -1, -1); var v100 = V(1, -1, -1); var v110 = V(1, 1, -1); var v010 = V(-1, 1, -1);
        var v001 = V(-1, -1, 1); var v101 = V(1, -1, 1); var v111 = V(1, 1, 1); var v011 = V(-1, 1, 1);
        // 아래 면
        DL(v000, v100, cAabb); DL(v100, v110, cAabb); DL(v110, v010, cAabb); DL(v010, v000, cAabb);
        // 위 면
        DL(v001, v101, cAabb); DL(v101, v111, cAabb); DL(v111, v011, cAabb); DL(v011, v001, cAabb);
        // 수직
        DL(v000, v001, cAabb); DL(v100, v101, cAabb); DL(v110, v111, cAabb); DL(v010, v011, cAabb);

        // === OverlapBox 실행 ===
        var hits = Physics.OverlapBox(aabb.center, aabb.extents, Quaternion.identity, mask, qti);
        Debug.Log($"[DragFrustum] OverlapBox found {hits.Length} colliders in AABB");

        var res = new List<Collider>(hits.Length);
        foreach (var col in hits)
        {
            if (!col) continue;

            // === 프러스텀 테스트 ===
            bool insideFrustum = GeometryUtility.TestPlanesAABB(planes, col.bounds);

            Debug.Log($"[DragFrustum] Testing {col.name}: Bounds={col.bounds.center}, Inside={insideFrustum}");

            if (!insideFrustum) continue;

            res.Add(col);
        }

        Debug.Log($"[DragFrustum] Final result: {res.Count} colliders passed frustum test");
        return res;
    }
}
