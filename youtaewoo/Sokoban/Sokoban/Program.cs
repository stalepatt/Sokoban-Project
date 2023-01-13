using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban
{
    enum Direction // 방향을 저장하는 타입
    {
        None,
        Left,
        Right,
        Up,
        Down
    }


    class Sokoban
    {
        static void Main()
        {
            // 초기 세팅
            Console.ResetColor(); // 컬러를 초기화 하는 것
            Console.CursorVisible = false; // 커서를 숨기기
            Console.Title = "홍성재의 파이어펀치"; // 타이틀을 설정한다.
            Console.BackgroundColor = ConsoleColor.DarkGreen; // 배경색을 설정한다.
            Console.ForegroundColor = ConsoleColor.Yellow; // 글꼴색을 설정한다.
            Console.Clear(); // 출력된 내용을 지운다.

            // 기호 상수 정의
            const int GOAL_COUNT = 2;
            const int BOX_COUNT = GOAL_COUNT;
            const int WALL_COUNT = 2;

            // 플레이어 위치를 저장하기 위한 변수
            //Player player = new Player();
            //player.X = 0;
            //player.Y = 0;
            //player.Direction = Direction.None;
            //player.icon = "P";
            //player.pushedBoxId = 0;

            int playerX = 0;
            int playerY = 0;

            // 플레이어의 이동 방향을 저장하기 위한 변수
            Direction moveDirection = Direction.None;

            // 플레이어가 무슨 박스를 밀고 있는지 저장하기 위한 변수
            int pushedBoxId = 0; // 1이면 박스1, 2면 박스2

            // 박스 위치를 저장하기 위한 변수
            int[] boxPositionsX = { 5, 7 };
            int[] boxPositionsY = { 5, 3 };

            // 벽 위치를 저장하기 위한 변수
            int[] wallPositionsX = { 7, 5 };
            int[] wallPositionsY = { 7, 3 };

            // 골 위치를 저장하기 위한 변수
            int[] goalPositionsX = { 9, 1 };
            int[] goalPositionsY = { 9, 2 };

            // 박스가 골 위에 있는지를 저장하기 위한 변수
            bool[] isBoxOnGoal = new bool[BOX_COUNT];

            // 게임 루프 구성
            while (true)
            {
                Render();

                // --------------------------------- ProcessInput -----------------------------------------
                ConsoleKey key = Console.ReadKey().Key;

                // --------------------------------- Update -----------------------------------------

                // 플레이어 이동 처리
                PlayerMove(key, ref playerX, ref playerY, ref moveDirection);

                // 플레이어와 벽의 충돌 처리 
                for (int wallId = 0; wallId < WALL_COUNT; ++wallId)
                {
                    if (false == isCollided(playerX, playerY, wallPositionsX[wallId], wallPositionsY[wallId]))
                    {
                        continue;
                    }

                    switch (moveDirection)
                    {
                        case Direction.Left:
                            playerX = wallPositionsX[wallId] + 1;
                            break;
                        case Direction.Right:
                            playerX = wallPositionsX[wallId] - 1;
                            break;
                        case Direction.Up:
                            playerY = wallPositionsY[wallId] + 1;
                            break;
                        case Direction.Down:
                            playerY = wallPositionsY[wallId] - 1;
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {moveDirection}");

                            return;
                    }
                    break;
                }



                // 박스 이동 처리
                // 플레이어가 박스를 밀었을 때라는 게 무엇을 의미하는가? => 플레이어가 이동했는데 플레이어의 위치와 박스 위치가 겹쳤다.
                for (int boxId = 0; boxId < BOX_COUNT; ++boxId)
                {
                    if (false == isCollided(playerX, playerY, boxPositionsX[boxId], boxPositionsY[boxId]))
                    {
                        continue;
                    }

                    // 박스를 민다. => 박스의 좌표를 바꾼다.
                    switch (moveDirection)
                    {
                        case Direction.Left:
                            boxPositionsX[boxId] = Math.Max(0, boxPositionsX[boxId] - 1);
                            playerX = boxPositionsX[boxId] + 1;
                            break;
                        case Direction.Right:
                            boxPositionsX[boxId] = Math.Min(boxPositionsX[boxId] + 1, 20);
                            playerX = boxPositionsX[boxId] - 1;
                            break;
                        case Direction.Up:
                            boxPositionsY[boxId] = Math.Max(0, boxPositionsY[boxId] - 1);
                            playerY = boxPositionsY[boxId] + 1;
                            break;
                        case Direction.Down:
                            boxPositionsY[boxId] = Math.Min(boxPositionsY[boxId] + 1, 10);
                            playerY = boxPositionsY[boxId] - 1;
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {moveDirection}");

                            return;
                    }

                    pushedBoxId = boxId;
                    break;

                }

                // 박스와 벽의 충돌 처리 
                for (int boxId = 0; boxId < BOX_COUNT; ++boxId)
                {
                    for (int wallId = 0; wallId < WALL_COUNT; ++wallId)
                    {
                        if (false == isCollided(boxPositionsX[boxId], boxPositionsY[boxId], wallPositionsX[wallId], wallPositionsY[wallId]))
                        {
                            continue;
                        }
                        switch (moveDirection)
                        {
                            case Direction.Left:
                                boxPositionsX[boxId] = wallPositionsX[wallId] + 1;
                                playerX = boxPositionsX[boxId] + 1;
                                break;
                            case Direction.Right:
                                boxPositionsX[boxId] = wallPositionsX[wallId] - 1;
                                playerX = boxPositionsX[boxId] - 1;
                                break;
                            case Direction.Up:
                                boxPositionsY[boxId] = wallPositionsY[wallId] + 1;
                                playerY = boxPositionsY[boxId] + 1;
                                break;
                            case Direction.Down:
                                boxPositionsY[boxId] = wallPositionsY[wallId] - 1;
                                playerY = boxPositionsY[boxId] - 1;
                                break;
                            default:
                                Console.Clear();
                                Console.WriteLine($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {moveDirection}");

                                return;
                        }

                        break;
                    }
                }


                // 박스끼리 충돌 처리
                for (int collidedBoxId = 0; collidedBoxId < BOX_COUNT; ++collidedBoxId)
                {
                    if (pushedBoxId == collidedBoxId)
                    {
                        continue;
                    }
                    if (false == isCollided(boxPositionsX[pushedBoxId], boxPositionsY[pushedBoxId], boxPositionsX[collidedBoxId], boxPositionsY[collidedBoxId]))
                    {
                        continue;
                    }

                    switch (moveDirection)
                    {
                        case Direction.Left:
                            boxPositionsX[pushedBoxId] = boxPositionsX[collidedBoxId] + 1;
                            playerX = boxPositionsX[pushedBoxId] + 1;

                            break;
                        case Direction.Right:
                            boxPositionsX[pushedBoxId] = boxPositionsX[collidedBoxId] - 1;
                            playerX = boxPositionsX[pushedBoxId] - 1;

                            break;
                        case Direction.Up:
                            boxPositionsY[pushedBoxId] = boxPositionsY[collidedBoxId] + 1;
                            playerY = boxPositionsY[pushedBoxId] + 1;

                            break;
                        case Direction.Down:
                            boxPositionsY[pushedBoxId] = boxPositionsY[collidedBoxId] - 1;
                            playerY = boxPositionsY[pushedBoxId] - 1;

                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine($"[Error] 플레이어 이동 방향 데이터가 오류입니다. : {moveDirection}");

                            return;
                    }

                    break;
                }




                // 박스와 골의 처리

                int boxOnGoalCount = 0;

                // 골 지점에 박스에 존재하냐?
                for (int goalId = 0; goalId < GOAL_COUNT; ++goalId) // 모든 골 지점에 대해서
                {
                    // 박스가 있는지 체크한다.
                    for (int boxId = 0; boxId < BOX_COUNT; ++boxId) // 모든 박스에 대해서
                    {
                        //박스가 골 지점 위에 있는지 확인한다.
                        if (boxPositionsX[boxId] == goalPositionsX[goalId] && boxPositionsY[boxId] == goalPositionsY[goalId])
                        {
                            ++boxOnGoalCount;
                            break;
                        }
                    }
                }
                // 모든 골 지점에 박스가 올라와 있다.
                if (boxOnGoalCount == GOAL_COUNT)
                {
                    Console.Clear();
                    Console.WriteLine("축하합니다. 클리어 하셨습니다.");

                    break;
                }
            }

            void RenderObject(int x, int y, string obj)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(obj);
            }

            // 프레임을 그린다.
            void Render()
            {
                // 이전 프레임을 지운다.
                Console.Clear();

                // 벽을 그린다.
                for (int wallId = 0; wallId < WALL_COUNT; ++wallId)
                {
                    RenderObject(wallPositionsX[wallId], wallPositionsY[wallId], "W");
                }

                // 골을 그린다.
                for (int goalId = 0; goalId < GOAL_COUNT; ++goalId)
                {
                    RenderObject(goalPositionsX[goalId], goalPositionsY[goalId], "G");
                }

                // 플레이어를 그린다.
                RenderObject(playerX, playerY, "P");

                // 박스를 그린다.
                for (int boxId = 0; boxId < BOX_COUNT; ++boxId)
                {
                    string boxShape = isBoxOnGoal[boxId] ? "O" : "B";
                    RenderObject(boxPositionsX[boxId], boxPositionsY[boxId], boxShape);
                }
            }

            void PlayerMove(ConsoleKey key, ref int x, ref int y, ref Direction moveDirection)
            {
                if (key == ConsoleKey.LeftArrow)
                {
                    x = Math.Max(0, x - 1);
                    moveDirection = Direction.Left;
                }

                if (key == ConsoleKey.RightArrow)
                {
                    x = Math.Min(x + 1, 20);
                    moveDirection = Direction.Right;
                }

                if (key == ConsoleKey.UpArrow)
                {
                    y = Math.Max(0, y - 1);
                    moveDirection = Direction.Up;
                }

                if (key == ConsoleKey.DownArrow)
                {
                    y = Math.Min(y + 1, 10);
                    moveDirection = Direction.Down;
                }
            }

            bool isCollided(int x1, int y1, int x2, int y2)
            {
                if (x1 == x2 && y1 == y2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}


