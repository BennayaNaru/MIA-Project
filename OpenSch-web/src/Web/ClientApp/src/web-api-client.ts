const belumTersambung = () =>
  Promise.reject(new Error("Backend belum tersambung"));

export class UsersClient {
  infoGET(..._args: any[]): Promise<any> { return belumTersambung(); }
  infoPOST(..._args: any[]): Promise<any> { return belumTersambung(); }
  login(..._args: any[]): Promise<any> { return belumTersambung(); }
  register(..._args: any[]): Promise<any> { return belumTersambung(); }
  logout(..._args: any[]): Promise<any> { return belumTersambung(); }
}

export class LoginRequest {
  constructor(data?: any) { Object.assign(this, data); }
}
export class RegisterRequest {
  constructor(data?: any) { Object.assign(this, data); }
}

export class WeatherForecastsClient {
  getWeatherForecasts(): Promise<any> { return belumTersambung(); }
}
export class TodoListsClient {
  get(): Promise<any> { return belumTersambung(); }
}
export class TodoItemsClient {  
  get(..._args: any[]): Promise<any> { return belumTersambung(); }
}