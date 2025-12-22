import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import os
import numpy as np
from matplotlib.patches import Patch

# ---------------------------------------------------------
# 1. 데이터 로드 및 전처리 설정
# ---------------------------------------------------------

# 파일 경로 설정 (사용자 환경에 맞게 수정 필요)
files = {
    'Controller_Origin': 'TestControllerAgent_Origin(Clone)1_vs_Enemy.xlsx',
    'Controller_Miss': 'TestControllerAgent_MissReward(Clone)1_vs_Enemy.xlsx',
    'Total_Origin': 'TestTotalAgent_Origin(Clone)1_vs_Enemy.xlsx',
    'Total_Miss': 'TestTotalAgent_MissReward(Clone)1_vs_Enemy.xlsx',
}

# 컬럼 정의 (Total Agent 파일에는 헤더가 없거나 불규칙하므로 명시적으로 지정)
cols = ['ID', 'Agent', 'Win', 'Success', 'Fail', 'Hit', 'Lose', 'Playtime']

def load_and_process_data(files):
    data_frames = []

    # 1. Controller Agent 데이터 로드 (헤더가 있는 Excel)
    for key in ['Controller_Origin', 'Controller_Miss']:
        try:
            df = pd.read_excel(files[key])
            df.columns = cols 
            df['AgentType'] = 'M' # Controller -> M
            df['RewardType'] = 'With Miss Penalty' if 'Miss' in key else 'Without Miss Penalty'
            data_frames.append(df)
        except Exception as e:
            print(f"Error loading {key}: {e}")

    # 2. Total Agent 데이터 로드 (헤더가 없는 경우 대비)
    for key in ['Total_Origin', 'Total_Miss']:
        try:
            df = pd.read_excel(files[key], header=None, names=cols)
            df = df[pd.to_numeric(df['ID'], errors='coerce').notna()]
            df['AgentType'] = 'T' # Total -> T
            df['RewardType'] = 'With Miss Penalty' if 'Miss' in key else 'Without Miss Penalty'
            data_frames.append(df)
        except Exception as e:
            print(f"Error loading {key}: {e}")

    # 데이터 병합
    df_all = pd.concat(data_frames, ignore_index=True)

    # 숫자형 데이터 변환
    numeric_cols = ['Win', 'Success', 'Fail', 'Playtime']
    for col in numeric_cols:
        df_all[col] = pd.to_numeric(df_all[col], errors='coerce')

    # 유효 데이터 필터링
    df_all = df_all.dropna(subset=['Win'])
    df_all = df_all[df_all['Win'].isin([0, 1])]

    # 파생 변수 계산
    df_all['Total Attacks'] = df_all['Success'] + df_all['Fail']
    df_all['Accuracy'] = (df_all['Success'] / df_all['Total Attacks']).fillna(0)
    df_all['TimeEfficiency'] = (df_all['Success'] / df_all['Playtime']).fillna(0)

    # 에이전트 레이블 생성
    conditions = [
        (df_all['AgentType'] == 'T') & (df_all['RewardType'] == 'Without Miss Penalty'),
        (df_all['AgentType'] == 'T') & (df_all['RewardType'] == 'With Miss Penalty'),
        (df_all['AgentType'] == 'M') & (df_all['RewardType'] == 'Without Miss Penalty'),
        (df_all['AgentType'] == 'M') & (df_all['RewardType'] == 'With Miss Penalty')
    ]
    choices = ['$T_1$', '$T_2$', '$M_1$', '$M_2$']
    df_all['AgentLabel'] = np.select(conditions, choices, default='')

    return df_all

# ---------------------------------------------------------
# 2. 그래프 그리기 (논문 스타일 - 해칭 수정)
# ---------------------------------------------------------

def plot_metrics(df):
    sns.set_style("whitegrid")
    order = ['$T_1$', '$T_2$', '$M_1$', '$M_2$']
    
    # 색상 팔레트 정의 (T: 파랑, M: 주황)
    palette = {
        '$T_1$': '#4C72B0', '$T_2$': '#4C72B0',
        '$M_1$': '#DD8452', '$M_2$': '#DD8452'
    }
    
    # 해칭 정의 (더 촘촘하게)
    hatches = ['', '///', '', '///']

    metrics = [
        {'y': 'Total Attacks', 'title': 'Total Attacks Count (Lower is Better)', 'ylabel': 'Count', 'filename': 'plot_total_attack_count.png'},
        {'y': 'Accuracy', 'title': 'Accuracy (Higher is Better)', 'ylabel': 'Accuracy (0~1)', 'filename': 'plot_accuracy.png'},
        {'y': 'Playtime', 'title': 'Playtime (Lower is Better)', 'ylabel': 'Time (Seconds)', 'filename': 'plot_playtime.png'},
        {'y': 'TimeEfficiency', 'title': 'Time Efficiency (Higher is Better)', 'ylabel': 'Success / Second', 'filename': 'plot_efficiency.png'}
    ]

    for m in metrics:
        plt.figure(figsize=(8, 6))
        ax = sns.barplot(
            data=df, x='AgentLabel', y=m['y'], order=order,
            palette=palette,
            capsize=0.1, linewidth=1.5, edgecolor='black',
            errcolor='black', errwidth=1
        )
        
        # 바에 해칭 적용
        for i, bar in enumerate(ax.patches):
            if hatches[i]:
                bar.set_hatch(hatches[i])
            
        # 레이블 및 타이틀
        plt.title(m['title'], fontsize=25, pad=20)
        plt.xlabel("Agent", fontsize=20)
        plt.ylabel(m['ylabel'], fontsize=20)
        plt.xticks(fontsize=20)
        plt.yticks(fontsize=20)
        
        # 커스텀 범례
        # legend_elements = [
        #     Patch(facecolor='#4C72B0', label='Total Agent ($T$)'),
        #     Patch(facecolor='#DD8452', label='Controller Agent ($M$)'),
        #     Patch(facecolor='white', edgecolor='black', hatch='///', label='With Miss Penalty')
        # ]
        # ax.legend(handles=legend_elements, fontsize=12, loc='best')

        plt.tight_layout()
        plt.savefig(m['filename'], dpi=300)
        print(f"Saved {m['filename']}")
        plt.show()

# ---------------------------------------------------------
# 실행
# ---------------------------------------------------------
if __name__ == "__main__":
    if os.path.exists(list(files.values())[0]):
        df = load_and_process_data(files)
        plot_metrics(df)
    else:
        print("파일을 찾을 수 없습니다. 경로를 확인해주세요.")