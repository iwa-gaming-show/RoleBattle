# RoleBattle

3種類のカードのどれかを場に出し勝敗を決め、獲得ポイントを競おう！必殺技を使用して勝つとポイントアップ！
<br>

## ゲーム説明

### ----カード相性----
  カードには相性が存在し、PRINCESS->BRAVE->DEVIL->PRINCESSといったそれぞれ矢印の方向に強いといった特性を持ちます。<br>

  <img width="400" alt="カード相性" src="https://user-images.githubusercontent.com/96030906/147873644-08fd9c5d-067e-4cda-a7c6-56d2a0b82e6d.png">

### ----進行----
自分のターンになったら手札の三種類のうち一枚を選択し場にカードを出します。
カードでのバトルで勝利するとポイントを獲得します。
1回の勝負を1ラウンドとして扱い、こちらを3回行います。

### ----必殺技----
1ゲームに1回のみ必殺技を発動することが可能です。
発動したラウンドでカードバトルに勝利するとそのラウンドの獲得ポイントが2倍になります。

### ----勝利条件----
3ラウンド経過後の獲得ポイント数が対戦相手より多い。


CPU戦、マルチ戦を実装。マルチ戦は今後新しい機能を随時追加していきます。

## 実装機能
- カード同士のじゃんけん、勝敗によるポイント獲得
- お互いの出したカードの裏から表にする演出
- ポイントの結果による勝敗の表示
- 必殺技演出
- CPU及び、PvPのマルチ対戦
- Bgm,SEの音量調整、設定データの保存
- プレイヤーの使用キャラクターの選択、変更
- etc...
## 技術目標
- PUN2を使ってマルチ対戦機能を実装してみる
- UniTaskを使って非同期処理を書いてみる
- DOTweenを使ってUIのアニメーションを作ってみる
- UnityTestRunnerを導入してみる(※導入のみでまともにテストは書けませんでした)
- クラス設計を意識し、参照関係を一方向にする
- ゲームのオプションなどのUIのシステム周りをこだわって作る
- Canvasの描画パフォーマンスを意識して作ってみる

## クラス図
簡易的なもので、UMLでもありませんが作ってみました。
各シーンごとに記載してあります。

![ソロ用](https://user-images.githubusercontent.com/96030906/147876876-0a72f5dc-6da0-4386-8a5b-9c9dfb1b767c.png)
<br>
![マルチ用](https://user-images.githubusercontent.com/96030906/147876899-14da16c8-8eff-4045-a048-859e957721fb.png)
<br>
![SuperClass_and_Singleton](https://user-images.githubusercontent.com/96030906/147876912-06a91982-bcd7-443b-b32a-e1a10f50f226.png)
<br>
![プレイヤーオプション](https://user-images.githubusercontent.com/96030906/147876950-e53e68ad-a9f4-4c44-b794-6b112f818d40.png)
<br>
## 反省
- クラス設計をもう少し考えてから開発に取り掛かるべきだった。<br>マルチ用(vsPlayer)のゲームを制作しようとした際、ソロ用のスクリプトを使い回そうと考えたが中々に難しく感じることが多かった。<br>原因を考えた結果、クラスごとの処理のフローがぐちゃぐちゃになっており、処理の流れを追うのが大変だったり、マルチ用にうまく当てはめることができず、同じ機能をマルチ用に作り直さなければいけないことがあった。<br>改善のため、ソロ用のクラス設計を一から見つめ直し作り直した結果、マルチ用ともうまく共通処理・クラスを使い回したり、処理の流れを整理することで見通しがよくなった。<br>初期段階でクラスごとの関係性や全体の処理フローを見通せていれば、このような手間をかけることもなかったため反省。

## 感想
 - 今回は技術(ライブラリや設計)の学習を目的に制作したが、完成したゲーム自体は簡単なもので面白いゲームが作れたかというと正直疑問が残る。今後はどうしたら面白いと思ってもらえるゲームが作れるかもっと考えつつ技術の上達にも力を入れていきたい。

## 今後追加したい機能
- プレイヤーの獲得ポイントの合計によるランキング実装
- キャラクターごとに必殺技の実装
- マルチ対戦での簡易メッセージ送信機能
- メッセージ機能を利用したじゃんけんの心理戦機能
- 対戦相手のこれまで出したカードの履歴を実装(対戦相手はこの履歴を見て傾向を確認し次の相手の出す手を読む)
