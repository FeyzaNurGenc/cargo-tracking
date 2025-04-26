export interface ResultModel<T>{
    data?: T,
    errormessages?: string[],
    statusCode: number,
    isSuccessful: boolean
}