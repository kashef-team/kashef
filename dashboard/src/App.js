import React, { Component } from 'react';
import { Admin, Resource } from 'react-admin';

import restProvider from './sailsRestProvider';
import authProvider from './authProvider';
import logo from './logo.svg';
import './App.css';

import { PersonList, PersonCreate, PersonEdit } from './persons';
import { CameraList, CameraCreate, CameraEdit } from './cameras';

const App = () => (
  <Admin
    dataProvider={restProvider('http://localhost:1337')}
    authProvider={authProvider}
    title="Kashef Dashboard"
  >
    <Resource name="person" list={PersonList} create={PersonCreate} edit={PersonEdit} />
    <Resource name="camera" list={CameraList} create={CameraCreate} edit={CameraEdit} />
  </Admin>
);

export default App;
