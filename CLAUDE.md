# CLAUDE.md

このファイルは、Claude Codeがこのリポジトリで作業する際に必ず参照するプロジェクト固有の指示書です。

## プロジェクト概要

- ゲームタイトル: **Project S.C.A.V.**
- ジャンル: 2Dプラットフォーマー × ローグライク × 脱出シューター
- 1プレイが数日規模のロングセッションを想定した設計（詳細は `概要.md`）
- Unity バージョン: **2022.3.24f1**

詳細な世界観・ゲームルール・強化要素は `概要.md` を参照すること。関連ドキュメント:

| ファイル | 内容 |
|---|---|
| `概要.md` | ゲーム全体のコンセプト・世界観・ゲームルール・プレイヤー強化要素 |
| `拠点.md` | 拠点（ホームベース）の設備・アップグレード・案件システム |
| `企業.md` | 企業（クエスト発注元）の設定・報酬 |
| `データベース管理方針.md` | データベース実装の設計方針（全件収集 vs キュレーション、Addressables運用） |

新機能を追加・修正する際は、まずこれらのドキュメントに矛盾しないか確認すること。ゲームデザインに関わる変更は、ドキュメント側の更新も合わせて提案する。

## アーキテクチャ方針

### MVP + pure C# Model

- 本プロジェクトは **MVP (Model-View-Presenter)** を基本方針とする。
- **Model層は pure C# で実装し、UnityEngineへの依存を持たせない。**
  - `Vector2` / `Vector2Int` 等のUnity構造体の使用は許容するが、`MonoBehaviour` / `Component` / `ScriptableObject` そのものへの参照、Unity API（`Physics2D`、`Input`、`Tilemap`等）の直接呼び出しは行わない。
  - Unity側の情報や機能が必要な場合は、**interfaceまたはFuncデリゲート越しに外部から注入**する。
    - 例: `NavGraphBuilder` は `Func<Vector2Int, bool> hasTile` を受け取り、`Tilemap` を直接参照しない。
    - 例: `EntityModel`/`PlayerModel`/`ChasePlayerEnemyModel` は `IMover`, `ILooker`, `IInputGetter`, `IEntityScanner`, `IProjectileSpawner`, `IPathfinder` 等のインターフェース越しにViewの機能を利用する。
- **View / Presenter は MonoBehaviour** として実装し、Unity固有の処理（入力取得、物理演算、描画、Tilemap走査等）を担当する。
- **Presenterは Model と View を繋ぐ薄い層**に徹する。Bind()でイベント購読するのみで、ロジックそのものは持たせない。
  ```csharp
  private void Bind()
  {
      _model.OnRequestMove += _view.Move;
  }
  ```
- Model → View/Presenter への通知は `event Action<T>` を使う。逆方向（View/Presenter → Model への指示）はメソッド呼び出しでよい。
- **親クラスのModelが子クラスのModelの参照を持つのは許容**する（例: `ChasePlayerEnemyModel` が交戦対象を `EntityModel _target` として保持する等）。逆に子が親の詳細を知る設計は避ける。

### データ層・データベース

- `ScriptableObject` の `XXData` から `CreateModel()` でpure C#の `XXModel` を生成するファクトリパターンを使う（`GunData.CreateModel()`、`ItemData.CreateModel()` 等を参照）。
- 新しいデータベースを追加する前に、必ずどちらの種類に該当するか判断する（詳細は `データベース管理方針.md`）。

  | 種類 | 例 | 実装方針 |
  |---|---|---|
  | 全件データベース（フォルダ内を機械的に全部集めればよい） | ItemDatabase, GunDatabase, EnemyDatabase, CraftRecipeDatabase | Addressablesラベルによる自動収集。`XXDatabaseModel`(pure C#) + `XXDatabaseLoader`(Unity依存/static) + `GameBootstrapper`で常駐公開 |
  | キュレーションされたデータベース（人が意図的に選ぶ必要がある） | RoomDatabase等 | 従来通り SO + `SerializeField` リストで手動アサイン |

  迷ったら「新しいアセットを追加したら自動的にゲームに登場してよいか」で判断する。YESなら全件データベース。
  実装時は `ItemDatabaseModel.cs` / `ItemDatabaseLoader.cs` / `GameBootstrapper.cs` を雛形にする。

## コーディング規約

### カプセル化：外部から変更できない書き方にする

すべてのフィールドは `private`（可能なら `readonly`）とし、外部へは読み取り専用のプロパティ経由でのみ公開する。

```csharp
// SerializeField
[SerializeField] private Sprite _sprite;
public Sprite Sprite => _sprite;

// コレクション
private readonly List<int> _scores;
public List<int> Scores => _scores; // 必要なら IReadOnlyList<int> でさらに強く制限する
public bool Bust => _scores.Bust();

// ReactiveProperty
private readonly ReactiveProperty<int> _chipProp;
public ReadOnlyReactiveProperty<int> ChipProp => _chipProp;
public int Chip => _chipProp.Value;
```

- setterを公開しない。値の変更が必要な場合は意図が分かるメソッド名を用意する（例: `StatValue.AddFlat()` / `RemoveFlat()`、`LoadoutModel.SwitchSlot()`）。
- Dictionary/Listなど参照型を公開する場合は `IReadOnlyList<T>` / `IReadOnlyDictionary<TK,TV>` を優先し、呼び出し側からの直接改変を防ぐ（例: `NavGraph.Nodes`, `NavNode.Edges`）。

### 命名・スタイル

- privateフィールド: `_camelCase`
- プロパティ・public メンバ: `PascalCase`
- 名前空間（`namespace`）は現状使用していないため、既存コードに合わせてフラットに保つ。
- コメント・XMLドキュメントコメント（`/// <summary>`）は日本語で記述する。
- ログ出力は `Debug.Log` ではなく `DevLog.Log / DevLog.Warning / DevLog.Error` を使う。
- 汎用的な処理・拡張メソッドは `Extensions.cs` にまとめる。このファイルはいつでも自由に参照・追記してよい（`Dice()`, `ChoiceWithWeight()`, `NormalDistribution()`, `Sample()` 等の既存拡張は積極的に再利用する）。

### インターフェース越しの依存

- ModelがView側の機能（入力・物理・描画等）に依存する際は、必ず専用interfaceを介す。
- 1つのView(MonoBehaviour)が複数のinterfaceを実装し、Presenterから個別にModelへ注入されるケースが多い（例: `EntityView` は `IEntityScanner, IProjectileSpawner, IPathfinder, IMover, ILooker` を実装）。

## 作業の進め方（重要）

- **ユーザーから仕様やクラス構造の設計案を提示された場合、すぐに実装に移らないこと。**
  1. まず設計案に対する疑問点・深く議論すべき点を指摘する（エッジケース、責務の分離、既存アーキテクチャとの整合性、拡張性など）。
  2. より良い代案があれば具体的に提案する。
  3. 議論を経てユーザーの合意が得られてから実装に着手する。
- 既存コードのスタイル・命名規則・アーキテクチャ方針（MVP、pure C# Model、カプセル化）を必ず踏襲する。
- 新規クラスを追加する際は、最初に以下を明確にする。
  - Model / View / Presenter のどの層に属するか
  - Unity依存を持ってよい層か（Model層なら持たせない）
  - 全件データベースかキュレーションされたデータベースか（データ追加の場合）
