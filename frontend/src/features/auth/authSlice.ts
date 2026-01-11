import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import type { UserInfo } from "../../types";

type AuthState = {
  token: string | null;
  user: UserInfo | null;
};

const initialState: AuthState = {
  token: localStorage.getItem("accessToken"),
  user: null
};

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    setToken: (state, action: PayloadAction<string | null>) => {
      state.token = action.payload;
    },
    setUser: (state, action: PayloadAction<UserInfo | null>) => {
      state.user = action.payload;
    },
    logout: (state) => {
      state.token = null;
      state.user = null;
    }
  }
});

export const { setToken, setUser, logout } = authSlice.actions;
export default authSlice.reducer;
