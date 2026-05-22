namespace MeasureTool
{
    internal class VentBlobSettings
    {
        public int Threshold = 40;

        // Region Blob 전용
        public VentThresholdMode ThresholdMode = VentThresholdMode.Adaptive;
        public int AdaptiveBlockSize = 51;   // 홀수, 국소 평균 창 크기
        public double AdaptiveC = 7.0;        // 평균에서 뺄 상수(클수록 보수적)
        public int MorphCloseSize = 11;          // 테두리 봉합 커널(끊김 메우기)
        public int RegionMargin = 25;            // ROI 둘레 여유(경계 전경 제거용)
        public double MinAreaFraction = 0.03;    // ROI 대비 최소 Vent 면적
        public double MaxAreaFraction = 0.97;    // ROI 대비 최대 Vent 면적
        public double SymmetryTolerancePx = 6.0; // 무게중심 vs MinAreaRect 중심 허용 차이
        public int HorizontalCloseWidth = 31;   // 가로 끊김 봉합 커널 폭 (B(2) 대응)
        public int BlobRefineCloseSize = 15;    // best blob 정제 Close 커널 (C800 내부 구멍 제거)
        public bool UseBilateralPreblur = true; // 노이즈 억제 전처리 (C800 대응)
        public bool UseBackgroundFlatten = true;   // 조명 음영 보정 (top-hat 유사)
        public double FlattenKernelScale = 0.9;     // 배경 추정 커널 크기 (ROI 높이 대비 배율)
    }
}