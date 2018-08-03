import React, { Component } from 'react';
import { withStyles } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import TextField from '@material-ui/core/TextField';
import Button from '@material-ui/core/Button';

import logo from './assets/kashef-logo.png';

const styles = {
    page: {
        backgroundColor: '#03a9f4',
        width: '100vw',
        height: '100vh',
        textAlign: 'center',
        margin: 'auto',
    },

    slogan: {
        fontSize: '1.5em',
        color: 'white',
    },

    header: {
        paddingTop: '40px',
    },

    textField: {
        width: 200,
    },

    form: {
        marginTop: '40px',
    },

    login: {
        marginTop: '20px',
    },
};

const Login = (props) => {
    const { classes } = props;
    return (
        <div className={classes.page}>
            <div className={classes.header}>
                <img src={logo} width='200vw' />
                <Typography variant="title" color="inherit" className={classes.slogan}>
                    Open Your Third Eye
                </Typography>
            </div>
            <div className={classes.form}>
                <TextField
                    id="name"
                    label="Name"
                    className={classes.textField}
                    margin="normal"
                    InputLabelProps={{
                        shrink: true,
                    }}
                />
                <br />
                <TextField
                    id="password"
                    label="Password"
                    className={classes.textField}
                    margin="normal"
                    type="password"
                    InputLabelProps={{
                        shrink: true,
                    }}
                />
                
                <br />
                
                <Button variant="contained" color="secondary" className={classes.login}>
                    Login
                </Button>
            </div>
        </div>
    );
  }
  
  export default withStyles(styles)(Login);