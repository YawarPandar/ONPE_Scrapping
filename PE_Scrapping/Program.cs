using System;
using PE_Scrapping.Funciones;
using YPandar.Common.Functional;

namespace PE_Scrapping
{
    class Program
    {
        static void Main()
        {
            string opt = string.Empty;
            string sel = string.Empty;
            string tip_pro = string.Empty;
            string mesa_sel = string.Empty;

            Console.Title = Constants.APP_TITLE;
            FunctionalHandler.WriteLines(new string[]
            {
                Messages.DOUBLE_LINE(),
                Constants.APP_TITLE.PadLeft(41, ' ')
            });
            FunctionalHandler.RepeatActionIf(
                () =>
                {
                    FunctionalHandler.WriteLines(new string[]{
                        Messages.DOUBLE_LINE(),
                        Messages.SELECT_PROCESS_INPUT,
                        Messages.FIRST_STAGE_OPTION,
                        Messages.SECOND_STAGE_OPTION,
                        Messages.DOUBLE_LINE(),
                        Messages.SELECT_OPTION_AND_ENTER
                    });
                    opt = FunctionalHandler.GetUserInput(Messages.WAIT_FOR_ANSWER);
                },
                () =>
                {
                    return !opt.Equals(Constants.ProcesarPrimeraV) && !opt.Equals(Constants.ProcesarSegundaV);
                }
            );
            FunctionalHandler.RepeatActionIf(
                () =>
                {
                    FunctionalHandler.WriteLines(new string[]
                    {
                        Messages.DOUBLE_LINE(),
                        Messages.SELECT_PROCESS_TYPE_INPUT,
                        Messages.TOTAL_PROCESS_OPTION,
                        Messages.PARTIAL_PROCESS_OPTION,
                        Messages.DOUBLE_LINE(),
                        Messages.SELECT_OPTION_AND_ENTER
                    });
                    tip_pro = FunctionalHandler.GetUserInput(Messages.WAIT_FOR_ANSWER);
                },
                () =>
                {
                    return !tip_pro.Equals(Constants.ProcesoTotal) && !tip_pro.Equals(Constants.ProcesoParcial);
                }
            );
            FunctionalHandler.ExecuteActionIf(
                () =>
                {
                    FunctionalHandler.RepeatActionIf(
                        () =>
                        {
                            FunctionalHandler.WriteLines(new string[]
                            {
                                Messages.DOUBLE_LINE(),
                                Messages.SELECT_PARTIAL_OPTION,
                                Messages.UBIGEO_OPTION,
                                Messages.TABLE_OPTION,
                                Messages.DOUBLE_LINE(),
                                Messages.SELECT_OPTION_AND_ENTER
                            });
                            sel = FunctionalHandler.GetUserInput(Messages.WAIT_FOR_ANSWER);
                        },
                        () =>
                        {
                            return !sel.Equals(Constants.ProcesoUbigeo) && !sel.Equals(Constants.ProcesoMesa);
                        }
                    );
                },
                () =>
                {
                    return tip_pro.Equals(Constants.ProcesoParcial);
                }
            );
            FunctionalHandler.ExecuteActionIf(
                () =>
                {
                    FunctionalHandler.WriteLines(new string[] { Messages.DOUBLE_LINE() });
                    FunctionalHandler.RepeatActionIf(
                        () =>
                        {
                            FunctionalHandler.WriteLines(new string[] { Messages.INPUT_TABLE_NUMBER });
                            mesa_sel = FunctionalHandler.GetUserInput(Messages.WAIT_FOR_ANSWER);
                        },
                        () =>
                        {
                            return string.IsNullOrEmpty(mesa_sel);
                        }
                    );
                },
                () =>
                {
                    return sel.Equals(Constants.ProcesoMesa);
                }
            );
            FunctionalHandler.ExecuteActionIf(
                () =>
                {
                    FunctionalHandler.WriteLines(new string[] { Messages.DOUBLE_LINE() });
                    FunctionalHandler.RepeatActionIf(
                        () =>
                        {
                            FunctionalHandler.WriteLines(new string[] { Messages.INPUT_UBIGEO_CODE });
                            mesa_sel = FunctionalHandler.GetUserInput(Messages.WAIT_FOR_ANSWER);
                        },
                        () =>
                        {
                            return string.IsNullOrEmpty(mesa_sel) || mesa_sel.Trim().Length > 6;
                        }
                    );
                },
                () =>
                {
                    return sel.Equals(Constants.ProcesoUbigeo);
                }
            );
            MainProcess.ExecuteProcess(opt, tip_pro, sel, mesa_sel);
            FunctionalHandler.WriteLines(new string[] {
                Messages.PROCESS_FINISHED,
                Messages.PRESS_ANY_KEY,
            });
            Console.ReadKey();
        }
    }
}
